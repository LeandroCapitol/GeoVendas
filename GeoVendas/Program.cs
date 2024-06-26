using GeoVendas;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        string connection = "Server=localhost;DataBase=GeoVendas;Uid=root;Pwd=master";

        using (MySqlConnection con = new MySqlConnection(connection))
        {
            con.Open();

            Console.WriteLine("Para adicionar um ou mais produtos, digite 1 e para atualizar, digite 2");
            string opcao = Console.ReadLine();

            Console.WriteLine("Cole os dados JSON dos produtos (pressione Enter duas vezes para finalizar):");

            string json = ReadMultiLineInput();

            if (!string.IsNullOrWhiteSpace(json))
                CriarAtualizarProdutos(con, json, opcao);
            else
                Console.WriteLine("Entrada de JSON vazia. Nenhum produto foi processado.");

            con.Close();
        }
    }

    static string ReadMultiLineInput()
    {
        List<string> linhas = new List<string>();
        string linha;

        while (!string.IsNullOrWhiteSpace(linha = Console.ReadLine()))
        {
            linhas.Add(linha);
        }

        return string.Join(Environment.NewLine, linhas);
    }

    static void CriarAtualizarProdutos(MySqlConnection con, string jsonInput, string opcao)
    {
        try
        {
            var produtos = JsonConvert.DeserializeObject<List<Produtos>>(jsonInput);

            foreach (var produto in produtos)
            {
                if (opcao == "1")
                {
                    try
                    {
                        string sql = "INSERT INTO estoque (produto, cor, tamanho, deposito, data_disponibilidade, quantidade) " +
                                           "VALUES (@produto, @cor, @tamanho, @deposito, @data_disponibilidade, @quantidade)";

                        using (MySqlCommand command = new MySqlCommand(sql, con))
                        {
                            command.Parameters.AddWithValue("@produto", produto.Produto);
                            command.Parameters.AddWithValue("@cor", produto.Cor);
                            command.Parameters.AddWithValue("@tamanho", produto.Tamanho);
                            command.Parameters.AddWithValue("@deposito", produto.Deposito);
                            command.Parameters.AddWithValue("@data_disponibilidade", DateTime.Parse(produto.Data_disponibilidade));
                            command.Parameters.AddWithValue("@quantidade", produto.Quantidade);

                            command.ExecuteNonQuery();
                        }

                        Console.WriteLine($"Produtos adicionados com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao adicionar produto: {ex.Message}");
                    }
                }
                else if (opcao == "2")
                {
                    try
                    {
                        Console.WriteLine($"Qual Id do Produto para atualizar?");
                        string id = Console.ReadLine();

                        string updateSql = "UPDATE estoque SET produto = @produto, cor = @cor, tamanho = @tamanho, " +
                                           "deposito = @deposito, data_disponibilidade = @data_disponibilidade, " +
                                           "quantidade = @quantidade " +
                                           "WHERE id = @id";

                        using (MySqlCommand command = new MySqlCommand(updateSql, con))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.Parameters.AddWithValue("@produto", produto.Produto);
                            command.Parameters.AddWithValue("@cor", produto.Cor);
                            command.Parameters.AddWithValue("@tamanho", produto.Tamanho);
                            command.Parameters.AddWithValue("@deposito", produto.Deposito);
                            command.Parameters.AddWithValue("@data_disponibilidade", DateTime.Parse(produto.Data_disponibilidade));
                            command.Parameters.AddWithValue("@quantidade", produto.Quantidade);

                            command.ExecuteNonQuery();
                        }

                        Console.WriteLine($"Produto com ID {id} atualizado com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao atualizar produto: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Opção inválida.");
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Erro ao processar o JSON: {ex.Message}");
        }
    }
}