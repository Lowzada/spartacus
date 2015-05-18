/*
The MIT License (MIT)

Copyright (c) 2014,2015 William Ivanski

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Data;
using MySql;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Mysql.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza o MySQL .NET Provider para acessar um SGBD MySQL.
    /// </summary>
    public class Mysql : Spartacus.Database.Generic
    {
        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Mysql"/>.
        /// </summary>
        /// <param name='p_server'>
        /// IP do servidor MySQL.
        /// </param>
        /// <param name='p_port'>
        /// Porta de conexão.
        /// </param>
        /// <param name='p_database'>
        /// Nome da base de dados ou schema.
        /// </param>
        /// <param name='p_user'>
        /// Usuário do MySQL.
        /// </param>
        /// <param name='p_password'>
        /// Senha do MySQL.
        /// </param>
        public Mysql(string p_server, string p_port, string p_database, string p_user, string p_password)
            : base(p_server, p_port, p_database, p_user, p_password)
        {
            this.v_connectionstring = "Persist Security Info=False;"
                + "Server=" + p_server + ";"
                + "Port=" + p_port + ";"
                + "Database=" + p_database + ";"
                + "Uid=" + p_user + ";"
                + "Pwd=" + p_password;
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do arquivo de banco de dados a ser criado.</param>
        public override void CreateDatabase(string p_name)
        {
        }

        /// <summary>
        /// Abra a conexão com o banco de dados, se esta for persistente (por exemplo, SQLite).
        /// </summary>
        public override void Open()
        {
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        /// <returns>Retorna uma <see cref="System.Data.DataTable"/> com os dados de retorno da consulta.</returns>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar a consulta.</exception>
        public override System.Data.DataTable Query(string p_sql, string p_tablename)
        {
            System.Data.DataTable v_table = null;
            MySql.Data.MySqlClient.MySqlDataReader v_reader;
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            System.Data.DataRow v_row;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();
                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(p_sql, v_mycon);
                    v_reader = v_mycmd.ExecuteReader();

                    v_table = new System.Data.DataTable(p_tablename);
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_table.Columns.Add(this.FixColumnName(v_reader.GetName(i)), typeof(string));

                    while (v_reader.Read())
                    {
                        v_row = v_table.NewRow();
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            v_row[i] = v_reader[i].ToString();
                        v_table.Rows.Add(v_row);
                    }

                    v_reader.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_table;
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// Utiliza um DataReader para buscar em blocos.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        /// <param name='p_startrow'>
        /// Número da linha inicial.
        /// </param>
        /// <param name='p_endrow'>
        /// Número da linha final.
        /// </param>
        public override System.Data.DataTable Query(string p_sql, string p_tablename, uint p_startrow, uint p_endrow)
        {
            System.Data.DataTable v_table = null;
            MySql.Data.MySqlClient.MySqlDataReader v_reader;
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            System.Data.DataRow v_row;
            uint v_currentrow;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();
                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(p_sql, v_mycon);
                    v_reader = v_mycmd.ExecuteReader();

                    v_table = new System.Data.DataTable(p_tablename);
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_table.Columns.Add(this.FixColumnName(v_reader.GetName(i)), typeof(string));

                    v_currentrow = 0;
                    while (v_reader.Read())
                    {
                        if (v_currentrow >= p_startrow && v_currentrow <= p_endrow)
                        {
                            v_row = v_table.NewRow();
                            for (int i = 0; i < v_reader.FieldCount; i++)
                                v_row[i] = v_reader[i].ToString();
                            v_table.Rows.Add(v_row);
                        }

                        if (v_currentrow > p_endrow)
                            break;

                        v_currentrow++;
                    }

                    v_reader.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_table;
        }

        /// <summary>
        /// Executa um código SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override void Execute(string p_sql)
        {
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_mycon);
                    v_mycmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }
        }

        /// <summary>
        /// Executa um código SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        /// <param name='p_verbose'>
        /// Se deve ser mostrado o código SQL no console.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override void Execute(string p_sql, bool p_verbose)
        {
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            string v_sql;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                    if (p_verbose)
                    {
                        System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Mysql.Execute:", System.DateTime.UtcNow);
                        System.Console.WriteLine(v_sql);
                        System.Console.WriteLine("--------------------------------------------------");
                    }

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(v_sql, v_mycon);
                    v_mycmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando um único dado de retorno em uma string.
        /// </summary>
        /// <returns>
        /// string com o dado de retorno.
        /// </returns>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override string ExecuteScalar(string p_sql)
        {
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            string v_ret;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_mycon);
                    v_ret = v_mycmd.ExecuteScalar().ToString();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_ret;
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando um único dado de retorno em uma string.
        /// </summary>
        /// <returns>
        /// string com o dado de retorno.
        /// </returns>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_verbose'>
        /// Se deve ser mostrado o código SQL no console.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override string ExecuteScalar(string p_sql, bool p_verbose)
        {
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            string v_sql, v_ret;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                    if (p_verbose)
                    {
                        System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Mysql.ExecuteScalar:", System.DateTime.UtcNow);
                        System.Console.WriteLine(v_sql);
                        System.Console.WriteLine("--------------------------------------------------");
                    }

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(v_sql, v_mycon);
                    v_ret = v_mycmd.ExecuteScalar().ToString();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_ret;
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados, se esta for persistente (por exemplo, SQLite).
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        /// Deleta um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do banco de dados a ser deletado.</param>
        public override void DropDatabase(string p_name)
        {
        }

        /// <summary>
        /// Deleta o banco de dados conectado atualmente.
        /// </summary>
        public override void DropDatabase()
        {
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        public override int Transfer(string p_query, string p_insert, Spartacus.Database.Generic p_destdatabase)
        {
            MySql.Data.MySqlClient.MySqlDataReader v_reader;
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            int v_transfered = 0;
            string v_insert;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();
                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(p_query, v_mycon);
                    v_reader = v_mycmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        v_insert = p_insert;
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            v_insert = v_insert.Replace("#" + this.FixColumnName(v_reader.GetName(i)).ToLower() + "#", v_reader[i].ToString());

                        p_destdatabase.Execute(v_insert);
                        v_transfered++;
                    }

                    v_reader.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_transfered;
        }
    }
}
