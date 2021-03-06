using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TesteApiConnect
{
    class UtilizadorDAO
    {
        private static MySqlConnection connection = new MySqlConnection(DataBaseConnector.builderLocalhost.ToString());

        public static int TypeUser(string email)
        {
            int typeUser = -1; // 0 - Cliente, 1 - Instrutor, 2 - Rececionista

            try
            {
                connection.Open();

                // Utilizador é Cliente, Instrutor ou Rececionista? --------------------------------------------

                string sqlCommand = "select hashPass from Cliente where email = @EMAIL";
                MySqlCommand command = new MySqlCommand(sqlCommand, connection);

                command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                command.Parameters["@EMAIL"].Value = email;

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    typeUser = 0;
                }
                else
                {
                    sqlCommand = "select hashPass from Instrutor where email = @EMAIL";
                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                    command.Parameters["@EMAIL"].Value = email;
                    
                    result = command.ExecuteScalar();

                    if (result != null)
                    {
                        typeUser = 1;
                    }
                    else
                    {
                        sqlCommand = "select hashPass from Rececionista where email = @EMAIL";
                        command = new MySqlCommand(sqlCommand, connection);

                        command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                        command.Parameters["@EMAIL"].Value = email;

                        result = command.ExecuteScalar();

                        if (result != null)
                        {
                            typeUser = 2;
                        }
                    }
                }

                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return typeUser;
        }

        public static InterfaceUtilizador GetUser(string email)
        {
            int typeUser = TypeUser(email); // 0 - Cliente, 1 - Instrutor, 2 - Rececionista

            if (typeUser == -1)
            {
                return null;
            }

            try
            {
                connection.Open();

                MySqlCommand command;
                string sqlCommand;

                switch (typeUser)
                {
                    // Cliente
                    case 0:
                        {
                            sqlCommand = "select * from Cliente where email = @EMAIL";
                            command = new MySqlCommand(sqlCommand, connection);

                            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                            command.Parameters["@EMAIL"].Value = email;

                            MySqlDataReader reader = command.ExecuteReader();

                            reader.Read();

                            Cliente user = new Cliente(email, reader.GetInt32(1), reader.GetString(2), reader.GetInt16(5),
                                reader.GetDateTime(4), reader.GetString(7), reader.GetString(6));

                            reader.Close();

                            connection.Close();

                            return user;
                        }
                    // Instrutor
                    case 1:
                        {
                            sqlCommand = "select * from Instrutor where email = @EMAIL";
                            command = new MySqlCommand(sqlCommand, connection);

                            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                            command.Parameters["@EMAIL"].Value = email;

                            MySqlDataReader reader = command.ExecuteReader();

                            reader.Read();

                            Instrutor user = new Instrutor(email, reader.GetInt32(1), reader.GetString(2),
                                reader.GetInt16(5), reader.GetDateTime(4), reader.GetString(6));

                            reader.Close();

                            connection.Close();

                            return user;
                        }
                    // Rececionista
                    case 2:
                        {
                            sqlCommand = "select * from Rececionista where email = @EMAIL";
                            command = new MySqlCommand(sqlCommand, connection);

                            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                            command.Parameters["@EMAIL"].Value = email;

                            MySqlDataReader reader = command.ExecuteReader();

                            reader.Read();

                            Rececionista user = new Rececionista(email, reader.GetInt32(1), reader.GetString(2), 
                                reader.GetInt16(5), reader.GetDateTime(4), reader.GetString(6));

                            reader.Close();

                            connection.Close();

                            return user;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }

        public static InterfaceUtilizador LogIn(string email, string hashPassInserida, string token)
        {
            DateTime today = DateTime.Now;
            DateTime time_to_expire = today.AddDays(5);
            InterfaceUtilizador user = null;

            int typeUser = TypeUser(email);  // 0 - Cliente, 1 - Instrutor, 2 - Rececionista

            if (typeUser == -1)
            {
                return null;
            }

            try
            {
                connection.Open();

                MySqlCommand command;
                string sqlCommand;

                switch (typeUser)
                {
                    // Cliente
                    case 0:
                    {
                        sqlCommand = "select * from Cliente where email = @EMAIL";
                        command = new MySqlCommand(sqlCommand, connection);

                        command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                        command.Parameters["@EMAIL"].Value = email;

                        MySqlDataReader reader = command.ExecuteReader();

                        reader.Read();
                        string hashUser = reader.GetString(3);

                        if (hashUser.Equals(hashPassInserida))
                        {
                            user = new Cliente(email, reader.GetInt32(1), reader.GetString(2),
                                reader.GetInt16(5), reader.GetDateTime(4), reader.GetString(7), reader.GetString(6));

                            // Adicionar o Cliente à tabela de utilizadores online...

                            reader.Close();

                            sqlCommand = "insert into UtilizadoresOnline values (@EMAIL, @TIME_TO_EXPIRE, @TOKEN)";
                            command = new MySqlCommand(sqlCommand, connection);

                            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                            command.Parameters["@EMAIL"].Value = email;

                            command.Parameters.Add(new MySqlParameter("@TIME_TO_EXPIRE", MySqlDbType.DateTime));
                            command.Parameters["@TIME_TO_EXPIRE"].Value = time_to_expire;

                            command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                            command.Parameters["@TOKEN"].Value = token;

                            command.ExecuteScalar();
                        }

                        reader.Close();
                        break;
                    }
                    // Instrutor
                    case 1:
                    {
                        sqlCommand = "select * from Instrutor where email = @EMAIL";
                        command = new MySqlCommand(sqlCommand, connection);

                        command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                        command.Parameters["@EMAIL"].Value = email;

                        MySqlDataReader reader = command.ExecuteReader();

                        reader.Read();
                        string hashUser = reader.GetString(3);

                        if (hashUser.Equals(hashPassInserida))
                        {
                            user = new Instrutor(email, reader.GetInt32(1), reader.GetString(2),
                                reader.GetInt16(5), reader.GetDateTime(4), reader.GetString(6));

                            reader.Close();

                            // Adicionar o Cliente à tabela de utilizadores online...
                            sqlCommand = "insert into UtilizadoresOnline values (@EMAIL, @TIME_TO_EXPIRE, @TOKEN)";
                            command = new MySqlCommand(sqlCommand, connection);

                            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                            command.Parameters["@EMAIL"].Value = email;

                            command.Parameters.Add(new MySqlParameter("@TIME_TO_EXPIRE", MySqlDbType.DateTime));
                            command.Parameters["@TIME_TO_EXPIRE"].Value = time_to_expire;

                            command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                            command.Parameters["@TOKEN"].Value = token;

                            command.ExecuteScalar();
                        }

                        reader.Close();
                        break;
                    }
                    // Rececionista
                    case 2:
                    {
                        sqlCommand = "select * from Rececionista where email = @EMAIL";
                        command = new MySqlCommand(sqlCommand, connection);

                        command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                        command.Parameters["@EMAIL"].Value = email;

                        MySqlDataReader reader = command.ExecuteReader();

                        reader.Read();
                        string hashUser = reader.GetString(3);

                        if (hashUser.Equals(hashPassInserida))
                        {
                            user = new Rececionista(email, reader.GetInt32(1), reader.GetString(2),
                                reader.GetInt16(5), reader.GetDateTime(4), reader.GetString(6));

                            reader.Close();

                            // Adicionar o Cliente à tabela de utilizadores online...
                            sqlCommand = "insert into UtilizadoresOnline values (@EMAIL, @TIME_TO_EXPIRE, @TOKEN)";
                            command = new MySqlCommand(sqlCommand, connection);

                            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                            command.Parameters["@EMAIL"].Value = email;

                            command.Parameters.Add(new MySqlParameter("@TIME_TO_EXPIRE", MySqlDbType.DateTime));
                            command.Parameters["@TIME_TO_EXPIRE"].Value = time_to_expire;

                            command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                            command.Parameters["@TOKEN"].Value = token;

                            command.ExecuteScalar();
                        }

                        reader.Close();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return user;
        }


        public static void LogOut(string token)
        {
            try
            {
                connection.Open();

                string sqlCommand = "delete from UtilizadoresOnline where token = @TOKEN";
                MySqlCommand command = new MySqlCommand(sqlCommand, connection);

                command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                command.Parameters["@TOKEN"].Value = token;

                command.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }
        }


        public static bool IsUserOnline(string token)
        {
            bool r = false;

            try
            {
                connection.Open();

                string sqlCommand = "select data_expirar from UtilizadoresOnline where token = @TOKEN";
                MySqlCommand command = new MySqlCommand(sqlCommand, connection);

                command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                command.Parameters["@TOKEN"].Value = token;

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    DateTime dataExp = Convert.ToDateTime(result);
                    DateTime atual = DateTime.Now;

                    if (dataExp.CompareTo(atual) > 0)
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }
            
            return r;
        }

        public string GetUserGivenToken(string token)
        {
            string res_email = null;

            try
            {
                // Abre a conexão à Base de Dados
                connection.Open();

                string sqlCommand = "select email from UtilizadoresOnline where token = @TOKEN";
                MySqlCommand command = new MySqlCommand(sqlCommand, connection);

                command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                command.Parameters["@TOKEN"].Value = token;

                res_email = Convert.ToString(command.ExecuteScalar());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                // Fecha a conexão à Base de Dados
                connection.Close();
            }

            return res_email;
        }

        /*
         * Recebe a interface do utilizador, o tipo de utilizador(0, 1 ou 2), ou seja, 
         * Cliente, Instrutor ou Rececionista
         * e recebe a hash da password
         */
        public static bool InsertUser(InterfaceUtilizador user, int type, string hashPass)
        {
            bool success = false;

            try
            {
                string sqlCommand;

                MySqlCommand command = null;

                // 0 - Cliente, 1 - Instrutor, 2 - Rececionista
                if (type == 0)
                {
                    Cliente u = (Cliente)user;

                    ExisteLocal(u.localidade);

                    connection.Open();

                    sqlCommand = "insert into Cliente (email, nif, nome, hashpass, data_nascimento, " +
                        "genero, categoria, localidade) " +
                        "select * from (select @EMAIL, @NIF, @NOME, @HASHPASS," +
                        "@DATA_NASCIMENTO, @GENERO, @CATEGORIA, @LOCALIDADE) as tmp " +
                        "where not exists (select email from Cliente " +
                        "where email = @EMAIL or nif = @NIF) limit 1";

                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add(new MySqlParameter("@HASHPASS", MySqlDbType.VarChar));
                    command.Parameters["@HASHPASS"].Value = hashPass;

                    u.IniParamSql(command);
                }
                else if (type == 1)
                {
                    Instrutor u = (Instrutor)user;

                    ExisteLocal(u.localidade);

                    connection.Open();

                    sqlCommand = "insert into Instrutor (email, nif, nome, hashpass, data_nascimento, " +
                        "genero, localidade) " +
                        "select * from (select @EMAIL, @NIF, @NOME, @HASHPASS," +
                        "@DATA_NASCIMENTO, @GENERO, @LOCALIDADE) as tmp " +
                        "where not exists (select email from Instrutor " +
                        "where email = @EMAIL or nif = @NIF) limit 1";

                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add(new MySqlParameter("@HASHPASS", MySqlDbType.VarChar));
                    command.Parameters["@HASHPASS"].Value = hashPass;

                    u.IniParamSql(command);
                }
                else if (type == 2)
                {
                    Rececionista u = (Rececionista)user;

                    ExisteLocal(u.localidade);

                    connection.Open();

                    sqlCommand = "insert into Rececionista (email, nif, nome, hashpass, data_nascimento, " +
                        "genero, localidade) " +
                        "select * from (select @EMAIL, @NIF, @NOME, @HASHPASS," +
                        "@DATA_NASCIMENTO, @GENERO, @LOCALIDADE) as tmp " +
                        "where not exists (select email from Rececionista " +
                        "where email = @EMAIL or nif = @NIF) limit 1";

                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add(new MySqlParameter("@HASHPASS", MySqlDbType.VarChar));
                    command.Parameters["@HASHPASS"].Value = hashPass;

                    u.IniParamSql(command);
                }

                if (command.ExecuteNonQuery() == 1)
                    success = true;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return success;
        }

        /*
         * Função que remove um utilizador e devolve um bool, true caso tenha sido removido 
         * ou false em caso contrario (não existe,...)
         */
        public static void RemoveUser(string email, char type)
        {
            try
            {
                connection.Open();

                string sqlCommand = "delete from UtilizadoresOnline where email = @EMAIL";
                MySqlCommand command = new MySqlCommand(sqlCommand, connection);
                command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                command.Parameters["@EMAIL"].Value = email;
                command.ExecuteScalar();
                string table = "";
                switch (type)
                {
                    case 'C':{ table = "Cliente"; CascadeRemoveCliente(email, connection); break;
                }
                    case 'I':{ table = "Instrutor"; CascadeRemoveInstrutor(email, connection); break; }
                    case 'R':table = "Rececionista"; break;
            }
                sqlCommand = "delete from " + table + " where email = @EMAIL";
            command = new MySqlCommand(sqlCommand, connection);
            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
            command.Parameters["@EMAIL"].Value = email;

            command.ExecuteScalar();
        }
            catch (Exception e){Console.WriteLine(e.ToString());}
            finally{connection.Close();}
        }

        private static void CascadeRemoveCliente(string email, MySqlConnection connection)
        {
            DeleteTableOn("Avaliaçao_Agendada", "Cliente_email", email, connection);
            DeleteTableOn("Clientes_na_AulaGrupo", "Cliente_email", email, connection);
            DeleteTableOn("Cliente_no_EspaçoGinasio", "Cliente_email", email, connection);
            DeleteTableOn("PlanoTreino_do_Cliente", "Cliente_email", email, connection);
            DeleteTableOn("PlanoAlimentar_do_Cliente", "Cliente_email", email, connection);

        }

        private static void CascadeRemoveInstrutor(string email, MySqlConnection connection)
        {
            DeleteTableOn("Clientes_na_AulaGrupo", "Instrutor_email", email, connection);
            DeleteTableOn("Aula_Grupo", "Instrutor_email", email, connection);
            DeleteTableOn("Avaliaçao_Agendada", "Instrutor_email", email, connection);

        }

        private static void DeleteTableOn(String table, String param, String pValue, MySqlConnection connection)
        {
            string parameter = "@" + param.ToUpper();
            string sqlCommand = "delete from " + table + " where " + param + " = " + parameter;
            MySqlCommand command = new MySqlCommand(sqlCommand, connection);
            command.Parameters.Add(new MySqlParameter(parameter, MySqlDbType.VarChar));
            command.Parameters[parameter].Value = pValue;
            command.ExecuteScalar();

        }


        /*
         * Caso a localidade não exista na base de dados, é necessário
         * inserir na tabela do codigo postal
         * Adicionamos como valor default para codigo posta o "0000",
         * pois não temos maneira de saber qual o vervadeiro valor
         */
        public static void ExisteLocal(string local)
        {
            try
            {
                connection.Open();

                    string sqlCommand = "insert into Codigo_Postal (localidade, codigo_postal) " +
                          "select * from (select @LOCALIDADE, @CODIGO_POSTAL) as tmp " +
                          "where not exists ( select localidade from Codigo_Postal " +
                          "where localidade = @LOCALIDADE) limit 1";

                    MySqlCommand command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add("@LOCALIDADE", MySqlDbType.VarChar);
                    command.Parameters["@LOCALIDADE"].Value = local;

                    command.Parameters.Add("@CODIGO_POSTAL", MySqlDbType.VarChar);
                    command.Parameters["@CODIGO_POSTAL"].Value = "0000";

                    command.ExecuteScalar();
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    connection.Close();
                }
            }

        public static void UpdateUser(InterfaceUtilizador user, int type, string hashPass)
        {
            try
            {
                MySqlCommand command;
                string sqlCommand;

                if (type == 0)
                {
                    Cliente u = (Cliente)user;
                    sqlCommand = "update Cliente set hashPass = @HASHPASS, data_nascimento = @DATA_NASCIMENTO, " +
                        "categoria = @CATEGORIA, localidade = @LOCALIDADE " +
                        "where email = @EMAIL";

                    /*
                     * Verfica se a Localidade inserida existe.
                     * Senão existir, adiciona à Base de Dados
                     */ 
                    ExisteLocal(user.GetLocalidade());

                    connection.Open();
                    
                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add("@HASHPASS", MySqlDbType.VarChar);
                    command.Parameters["@HASHPASS"].Value = hashPass;

                    command.Parameters.Add("@DATA_NASCIMENTO", MySqlDbType.DateTime);
                    command.Parameters["@DATA_NASCIMENTO"].Value = u.data_nascimento.ToString("yyyy-MM-dd HH:mm:ss");

                    command.Parameters.Add("@CATEGORIA", MySqlDbType.VarChar);
                    command.Parameters["@CATEGORIA"].Value = u.categoria;

                    command.Parameters.Add("@LOCALIDADE", MySqlDbType.VarChar);
                    command.Parameters["@LOCALIDADE"].Value = u.localidade;

                    command.Parameters.Add("@EMAIL", MySqlDbType.VarChar);
                    command.Parameters["@EMAIL"].Value = u.email;

                    command.ExecuteScalar();
                }
                else if (type == 1)
                {
                    Instrutor u = (Instrutor)user;
                    sqlCommand = "update Instrutor set hashPass = @HASHPASS, data_nascimento = @DATA_NASCIMENTO, " +
                        "localidade = @LOCALIDADE " +
                        "where email = @EMAIL";

                    /*
                     * Verfica se a Localidade inserida existe.
                     * Senão existir, adiciona à Base de Dados
                     */
                    ExisteLocal(user.GetLocalidade());

                    connection.Open();

                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add("@HASHPASS", MySqlDbType.VarChar);
                    command.Parameters["@HASHPASS"].Value = hashPass;

                    command.Parameters.Add("@DATA_NASCIMENTO", MySqlDbType.DateTime);
                    command.Parameters["@DATA_NASCIMENTO"].Value = u.data_nascimento.ToString("yyyy-MM-dd HH:mm:ss");

                    command.Parameters.Add("@LOCALIDADE", MySqlDbType.VarChar);
                    command.Parameters["@LOCALIDADE"].Value = u.localidade;

                    command.Parameters.Add("@EMAIL", MySqlDbType.VarChar);
                    command.Parameters["@EMAIL"].Value = u.email;

                    command.ExecuteScalar();
                }
                else if (type == 2)
                {
                    Rececionista u = (Rececionista)user;
                    sqlCommand = "update Rececionista set hashPass = @HASHPASS, data_nascimento = @DATA_NASCIMENTO, " +
                        "localidade = @LOCALIDADE " +
                        "where email = @EMAIL";

                    /*
                     * Verfica se a Localidade inserida existe.
                     * Senão existir, adiciona à Base de Dados
                     */
                    ExisteLocal(user.GetLocalidade());

                    connection.Open();

                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add("@HASHPASS", MySqlDbType.VarChar);
                    command.Parameters["@HASHPASS"].Value = hashPass;

                    command.Parameters.Add("@DATA_NASCIMENTO", MySqlDbType.DateTime);
                    command.Parameters["@DATA_NASCIMENTO"].Value = u.data_nascimento.ToString("yyyy-MM-dd HH:mm:ss");

                    command.Parameters.Add("@LOCALIDADE", MySqlDbType.VarChar);
                    command.Parameters["@LOCALIDADE"].Value = u.localidade;

                    command.Parameters.Add("@EMAIL", MySqlDbType.VarChar);
                    command.Parameters["@EMAIL"].Value = u.email;

                    command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        public static List<string> GetAllEmails()
        {

            List<string> emailsList = new List<string>();

            try
            {

                connection.Open();

                string sqlCommand = "select email from Rececionista " +
                                     "union select email from Cliente " +
                                     "union select email from Instrutor";

                MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    emailsList.Add(reader.GetString(0));
                }

                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return emailsList;
        }

    }
}
