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
using System.Data.OleDb;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Oledb.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza a implementação OLE DB para acessar qualquer SGBD.
    /// </summary>
    public class Oledb : Spartacus.Database.Generic
    {
        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Oledb"/>.
        /// Cria a string de conexão ao banco.
        /// </summary>
        /// <param name='p_provider'>
        /// SGBD que fornece o banco de dados.
        /// </param>
        /// <param name='p_host'>
        /// Hostname ou IP onde o banco de dados está localizado.
        /// </param>
        /// <param name='p_port'>
        /// Porta TCP para conectar-se ao SGBG.
        /// </param>
        /// <param name='p_service'>
        /// Nome do serviço que representa o banco ao qual desejamos nos conectar.
        /// </param>
        /// <param name='p_user'>
        /// Usuário ou schema para se conectar ao banco de dados.
        /// </param>
        /// <param name='p_password'>
        /// A senha do usuário ou schema.
        /// </param>
        public Oledb (string p_provider, string p_host, string p_port, string p_service, string p_user, string p_password)
            : base(p_host, p_port, p_service, p_user, p_password)
        {
            Spartacus.Utils.File v_file;

            switch (p_provider)
            {
                case "Oracle":
                    this.v_connectionstring = "Provider=OraOLEDB.Oracle;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST="
                        + this.v_host + ")(PORT="
                        + this.v_port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME="
                        + this.v_service + ")));User Id="
                        + this.v_user + ";Password="
                        + this.v_password;
                    break;
                case "Access":
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, v_service);
                    if (v_file.v_extension.ToLower() == "accdb")
                        this.v_connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.v_service + ";Persist Security Info=False;";
                    else
                        this.v_connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.v_service + ";User Id=admin;Password=;";
                    break;
                default:
                    this.v_connectionstring = "Provider="
                        + p_provider + ";Addr="
                        + this.v_host + ";Port="
                        + this.v_port + ";Database="
                        + this.v_service + ";User Id="
                        + this.v_user + ";Password="
                        + this.v_password;
                    break;
            }
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
            System.Data.OleDb.OleDbDataReader v_reader;
            System.Data.OleDb.OleDbCommand v_olecmd;
            System.Data.DataRow v_row;

            using (System.Data.OleDb.OleDbConnection v_olecon = new System.Data.OleDb.OleDbConnection(this.v_connectionstring))
            {
                try
                {
                    v_olecon.Open();
                    v_olecmd = new System.Data.OleDb.OleDbCommand(p_sql, v_olecon);
                    v_reader = v_olecmd.ExecuteReader();

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
                catch (System.Data.OleDb.OleDbException e)
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
            System.Data.OleDb.OleDbDataReader v_reader;
            System.Data.OleDb.OleDbCommand v_olecmd;
            System.Data.DataRow v_row;
            uint v_currentrow;

            using (System.Data.OleDb.OleDbConnection v_olecon = new System.Data.OleDb.OleDbConnection(this.v_connectionstring))
            {
                try
                {
                    v_olecon.Open();
                    v_olecmd = new System.Data.OleDb.OleDbCommand(p_sql, v_olecon);
                    v_reader = v_olecmd.ExecuteReader();

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
                catch (System.Data.OleDb.OleDbException e)
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
            System.Data.OleDb.OleDbCommand v_olecmd;

            using (System.Data.OleDb.OleDbConnection v_olecon = new System.Data.OleDb.OleDbConnection(this.v_connectionstring))
            {
                try
                {
                    v_olecon.Open();

                    v_olecmd = new System.Data.OleDb.OleDbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_olecon);
                    v_olecmd.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException e)
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
            System.Data.OleDb.OleDbCommand v_olecmd;
            string v_sql;

            using (System.Data.OleDb.OleDbConnection v_olecon = new System.Data.OleDb.OleDbConnection(this.v_connectionstring))
            {
                try
                {
                    v_olecon.Open();

                    v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                    if (p_verbose)
                    {
                        System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Oledb.Execute:", System.DateTime.UtcNow);
                        System.Console.WriteLine(v_sql);
                        System.Console.WriteLine("--------------------------------------------------");
                    }

                    v_olecmd = new System.Data.OleDb.OleDbCommand(v_sql, v_olecon);
                    v_olecmd.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException e)
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
            System.Data.OleDb.OleDbCommand v_olecmd;
            string v_ret;

            using (System.Data.OleDb.OleDbConnection v_olecon = new System.Data.OleDb.OleDbConnection(this.v_connectionstring))
            {
                try
                {
                    v_olecon.Open();

                    v_olecmd = new System.Data.OleDb.OleDbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_olecon);
                    v_ret = v_olecmd.ExecuteScalar().ToString();
                }
                catch (System.Data.OleDb.OleDbException e)
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
            System.Data.OleDb.OleDbCommand v_olecmd;
            string v_sql, v_ret;

            using (System.Data.OleDb.OleDbConnection v_olecon = new System.Data.OleDb.OleDbConnection(this.v_connectionstring))
            {
                try
                {
                    v_olecon.Open();

                    v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                    if (p_verbose)
                    {
                        System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Oledb.ExecuteScalar:", System.DateTime.UtcNow);
                        System.Console.WriteLine(v_sql);
                        System.Console.WriteLine("--------------------------------------------------");
                    }

                    v_olecmd = new System.Data.OleDb.OleDbCommand(v_sql, v_olecon);
                    v_ret = v_olecmd.ExecuteScalar().ToString();
                }
                catch (System.Data.OleDb.OleDbException e)
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
    }
}
