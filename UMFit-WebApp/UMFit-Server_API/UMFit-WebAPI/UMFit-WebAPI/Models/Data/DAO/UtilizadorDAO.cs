
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using UMFit_WebAPI.Models.Security;
using UMFit_WebAPI.Models.UMFit_LN.Utilizadores;
using UMFit_WebAPI.Models.UMFit_LN.Utilizadores.Interfaces;

namespace UMFit_WebAPI.Models.Data.DAO
{
    public class UtilizadorDAO
    {
        private static MySqlConnection connection = new MySqlConnection(DataBaseConnector.builderLocalhost.ToString());

        public int TypeUser(string email)
        {
            int typeUser = -1; // 0 - Cliente, 1 - Instrutor, 2 - Rececionista

            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();

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

        public InterfaceUtilizador GetUser(string email)
        {
            int typeUser = TypeUser(email); // 0 - Cliente, 1 - Instrutor, 2 - Rececionista

            if (typeUser == -1)
            {
                return null;
            }

            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();

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

        public InterfaceUtilizador LogIn(string email, string passInserida, string token)
        {
            DateTime today = DateTime.Now;
            DateTime time_to_expire = today.AddDays(5);

            int typeUser = TypeUser(email);  // 0 - Cliente, 1 - Instrutor, 2 - Rececionista

            if (typeUser == -1)
            {
                return null;
            }

            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();

                string hashPass = CalculateHash.GetHashString(passInserida);

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

                        if (hashUser.Equals(hashPass))
                        {
                            Cliente user = new Cliente(email, reader.GetInt32(1), reader.GetString(2),
                                reader.GetInt16(5),
                                reader.GetDateTime(4), reader.GetString(7), reader.GetString(6));

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

                            return user;
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

                        if (hashUser.Equals(hashPass))
                        {
                            Instrutor user = new Instrutor(email, reader.GetInt32(1), reader.GetString(2),
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

                            return user;
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

                        if (hashUser.Equals(hashPass))
                        {
                            Rececionista user = new Rececionista(email, reader.GetInt32(1), reader.GetString(2),
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

                            return user;
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

            return null;
        }


        public void LogOut(string token)
        {
            if(connection.State == ConnectionState.Closed) connection.Open();

            string sqlCommand = "delete from UtilizadoresOnline where token = @TOKEN";
            MySqlCommand command = new MySqlCommand(sqlCommand, connection);

            command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
            command.Parameters["@TOKEN"].Value = token;

            command.ExecuteScalar();
            connection.Close();
        }


        public bool IsUserOnline(string token)
        {
            if(connection.State == ConnectionState.Closed) connection.Open();

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
                    connection.Close();
                    return true;
                }
                else
                {
                    connection.Close();
                    return false;
                }
            }

            connection.Close();

            return false;
        }

        public string GetUserGivenToken(string token)
        {
            try
            {
                // Abre a conexão à Base de Dados
                if(connection.State == ConnectionState.Closed) connection.Open();

                string sqlCommand = "select email from UtilizadoresOnline where token = @TOKEN";
                MySqlCommand command = new MySqlCommand(sqlCommand, connection);

                command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                command.Parameters["@TOKEN"].Value = token;

                string res_email = Convert.ToString(command.ExecuteScalar());

                // Fecha a conexão à Base de Dados
                connection.Close();

                return res_email;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }

        /*
         * Recebe a interface do utilizador, o tipo de utilizador(0, 1 ou 2), ou seja, 
         * Cliente, Instrutor ou Rececionista
         * e recebe a hash da password
         */
        public bool InsertUser(InterfaceUtilizador user, int type, string hashPass)
        {
            object res = false;
            
            try
            {
                string sqlCommand;

                MySqlCommand command = null;

                // 0 - Cliente, 1 - Instrutor, 2 - Rececionista
                if (type == 0)
                {
                    Cliente u = (Cliente)user;

                    ExisteLocal(u.localidade);

                    if(connection.State == ConnectionState.Closed) connection.Open();

                    sqlCommand = "insert into Cliente (email, nif, nome, hashpass, data_nascimento, " +
                        "genero, categoria, localidade) " +
                        "select * from (select @EMAIL as em, @NIF as ni, @NOME as nom, @HASHPASS as hashp," + 
                        "@DATA_NASCIMENTO as dat, @GENERO as gen, @CATEGORIA as cat, @LOCALIDADE as loc" +
                        ") as tmp " +
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

                    if(connection.State == ConnectionState.Closed) connection.Open();

                    sqlCommand = "insert into Instrutor (email, nif, nome, hashpass, data_nascimento, " +
                                 "genero, localidade) " +
                                 "select * from (select @EMAIL as em, @NIF as ni, @NOME as nom, @HASHPASS as hashp," + 
                                 "@DATA_NASCIMENTO as dat, @GENERO as gen, @LOCALIDADE as loc" +
                                 ") as tmp " +
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

                    if(connection.State == ConnectionState.Closed) connection.Open();

                    sqlCommand = "insert into Rececionista (email, nif, nome, hashpass, data_nascimento, " +
                                 "genero, localidade) " +
                                 "select * from (select @EMAIL as em, @NIF as ni, @NOME as nom, @HASHPASS as hashp," + 
                                 "@DATA_NASCIMENTO as dat, @GENERO as gen, @LOCALIDADE as loc" +
                                 ") as tmp " +
                                 "where not exists (select email from Rececionista " +
                                 "where email = @EMAIL or nif = @NIF) limit 1";

                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add(new MySqlParameter("@HASHPASS", MySqlDbType.VarChar));
                    command.Parameters["@HASHPASS"].Value = hashPass;

                    u.IniParamSql(command);
                }

                res = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return res.ToString().Equals("1") ? true : false;
        }    

        /*
         * Caso a localidade não exista na base de dados, é necessário
         * inserir na tabela do codigo postal
         * Adicionamos como valor default para codigo posta o "0000",
         * pois não temos maneira de saber qual o vervadeiro valor
         */
        public void ExisteLocal(string local)
        {
            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();

                string sqlCommand = "insert into Codigo_Postal (localidade, codigo_postal) " +
                                    "select * from (select @LOCALIDADE as loc, @CODIGO_POSTAL as cod) as tmp " +
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

        public void RenovaToken(string token)
        {

            try
            {
                //Renovar token date
                DateTime newDate = DateTime.Now.AddDays(5);
               
                if(connection.State == ConnectionState.Closed) connection.Open();

                string sqlCommand = "update UtilizadoresOnline set data_expirar = @DATA where token = @TOKEN";

                MySqlCommand command = new MySqlCommand(sqlCommand, connection);

                command.Parameters.Add(new MySqlParameter("@TOKEN", MySqlDbType.VarChar));
                command.Parameters["@TOKEN"].Value = token;

                command.Parameters.Add(new MySqlParameter("@DATA", MySqlDbType.DateTime));
                command.Parameters["@DATA"].Value = newDate;

                command.ExecuteScalar();
                
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        public List<string> GetAllEmails()
        {

            List<string> emailsList = new List<string>();

            try
            {

                if(connection.State == ConnectionState.Closed) connection.Open();

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

        public static void RemoveUser(string email,char type)
        {
            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();

                string sqlCommand = "delete from UtilizadoresOnline where email = @EMAIL";
                MySqlCommand command = new MySqlCommand(sqlCommand, connection);
                command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
                command.Parameters["@EMAIL"].Value = email;
                command.ExecuteScalar();
                string table="";
                switch (type)
                {
                    case 'C':{ table = "Cliente"; CascadeRemoveCliente(email,connection);  break;}
                    case 'I':{table = "Instrutor"; CascadeRemoveInstrutor(email,connection); break;}
                    case 'R':table = "Rececionista";break;
                }
                sqlCommand = "delete from "+ table +" where email = @EMAIL";
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
            DeleteTableOn("Avaliaçao_Agendada","Cliente_email",email,connection);
            DeleteTableOn("Clientes_na_AulaGrupo","Cliente_email",email,connection);
            DeleteTableOn("Cliente_no_EspaçoGinasio","Cliente_email",email,connection);    
            DeleteTableOn("PlanoTreino_do_Cliente","Cliente_email",email,connection);
            DeleteTableOn("PlanoAlimentar_do_Cliente","Cliente_email",email,connection);

        }
        
        private static void CascadeRemoveInstrutor(string email, MySqlConnection connection)
        {
            DeleteTableOn("Clientes_na_AulaGrupo","Instrutor_email",email,connection);
            DeleteTableOn("Aula_Grupo","Instrutor_email",email,connection);
            DeleteTableOn("Avaliaçao_Agendada","Instrutor_email",email,connection);

        }

        private static void DeleteTableOn(String table,String param,String pValue,MySqlConnection connection)
        {
            string parameter ="@"+ param.ToUpper();
            string sqlCommand = "delete from "+table+" where "+param+" = "+parameter;
            MySqlCommand command = new MySqlCommand(sqlCommand, connection);
            command.Parameters.Add(new MySqlParameter(parameter, MySqlDbType.VarChar));
            command.Parameters[parameter].Value = pValue;
            command.ExecuteScalar();
            
        }
        
        public void UpdateUser(InterfaceUtilizador user, int type, string hashPass)
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

                    if(connection.State == ConnectionState.Closed) connection.Open();
                    
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

                    if(connection.State == ConnectionState.Closed) connection.Open();

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

                    if(connection.State == ConnectionState.Closed) connection.Open();

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

        public void UpdateCat(string email, string cat)
        {
            try
            {   if(connection.State == ConnectionState.Closed) connection.Open();
                MySqlCommand command;
                string sqlCommand;

                    sqlCommand = "update Cliente set categoria = @CATEGORIA where email = @EMAIL";
                    
                    command = new MySqlCommand(sqlCommand, connection);

                    command.Parameters.Add("@CATEGORIA", MySqlDbType.VarChar);
                    command.Parameters["@CATEGORIA"].Value = cat;

                    command.Parameters.Add("@EMAIL", MySqlDbType.VarChar);
                    command.Parameters["@EMAIL"].Value = email;

                    command.ExecuteScalar();
            }
            catch(Exception e) {Console.WriteLine(e.ToString());}
            finally {connection.Close();}
        }

        public List<string> GetUserEmails()
        {

            List<string> emailsList = new List<string>();

            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();
                string sqlCommand = "select email from Cliente ";
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
        
        public List<string> GetClientesPremiumEmails()
        {

            List<string> emailsList = new List<string>();

            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();
                string sqlCommand = "select email, categoria from Cliente where categoria = 'Premium' ";
                MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(1).Equals("Premium"))
                    {
                        
                        emailsList.Add(reader.GetString(0));
                    }
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
        
        public List<string> GetInstrutorEmails()
        {

            List<string> emailsList = new List<string>();

            try
            {
                if(connection.State == ConnectionState.Closed) connection.Open();
                string sqlCommand = "select email from Instrutor ";
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
        
        public Dictionary<string,string> GetAllEmailsNames(string tipo)
        {

            Dictionary<string,string> emailsList = new Dictionary<string,string>();
            try
            {

                string sqlCommand;
                if (tipo.Equals("todos"))
                    sqlCommand = "select email, nome from Rececionista " +
                                 "union select email, nome from Cliente " +
                                 "union select email, nome from Instrutor";
                else
                {
                    sqlCommand = "select email, nome from " + tipo;}
                if(connection.State == ConnectionState.Closed) connection.Open();


                MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
                
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    emailsList.Add(reader.GetString(0), reader.GetString(1));
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

        public string GetHashPass(string email, int type)
        {
            
            if(connection.State == ConnectionState.Closed) connection.Open();

            string sqlCommand = null;
            switch (type)
            {
                case 0 : 
                    sqlCommand = "select hashPass from Cliente where email = @EMAIL";
                    break;
                case 1 :
                    sqlCommand = "select hashPass from Instrutor where email = @EMAIL";
                    break;
                case 2 : 
                    sqlCommand = "select hashPass from Rececionista where email = @EMAIL";
                    break;
                    
            }
            MySqlCommand command = new MySqlCommand(sqlCommand, connection);

            command.Parameters.Add(new MySqlParameter("@EMAIL", MySqlDbType.VarChar));
            command.Parameters["@EMAIL"].Value = email;

            return command.ExecuteScalar().ToString();
        }
        
        
    }
}
