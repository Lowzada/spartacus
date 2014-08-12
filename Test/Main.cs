using System;
using Spartacus;

namespace Test
{
    class MainClass
    {
        // objeto de conexao com bancos de dados
        static Spartacus.Database.Generic v_database;

        public static void Main(string[] args)
        {
            CryptoFileTest(args[0], args[1], args[2]);
        }

        #region DATABASE

        private static void DatabaseTest()
        {
            //v_database = new Spartacus.Database.Odbc("tpprodu", "pscore", "plaservcore");
            //v_database = new Spartacus.Database.Firebird("localhost", "3050", "/work/clientes/escriba/sanmod.gdb", "SYSDBA", "masterkey");
            //v_database = new Spartacus.Database.Mysql("localhost", "3306", "wassina3_00555", "root", "knightroot");
            //v_database = new Spartacus.Database.Oledb("Oracle", "127.0.0.1", "1521", "XE", "pscore", "plaservcore");
            //v_database = new Spartacus.Database.Sqlite("teste.db");
            v_database = new Spartacus.Database.Postgresql("localhost", "5432", "test", "postgres", "postgres123");

            try
            {
                v_database.Connect();
                System.Console.WriteLine("Conseguiu se conectar!");
            }
            catch (Spartacus.Database.Exception e)
            {
                System.Console.WriteLine(e.v_message);
            }
        }

        #endregion

        #region FILE EXPLORER

        private static void FileTest()
        {
            Spartacus.Utils.FileExplorer v_explorer;
            string v_line;
            char[] v_separator;
            bool listar = true;

            v_explorer = new Spartacus.Utils.FileExplorer("/home/william");

            try
            {
                System.Console.WriteLine(v_explorer.v_current.CompleteFileName());
                System.Console.WriteLine();

                v_explorer.List();

                foreach (Spartacus.Utils.File v_file in v_explorer.v_files)
                {
                    if (v_file.v_filetype == Spartacus.Utils.FileType.DIRECTORY)
                        System.Console.Write("D\t");
                    else
                        System.Console.Write("A\t");

                    System.Console.WriteLine("{0}\t{1}", v_file.v_id, v_file.v_name);
                }
                System.Console.WriteLine();
            }
            catch(Spartacus.Utils.Exception e)
            {
                System.Console.WriteLine(e.v_message);
            }

            v_separator = new char[1];
            v_separator[0] = ' ';

            v_line = System.Console.ReadLine();
            while (v_line != "exit")
            {
                switch(v_line.Substring(0, 1))
                {
                    case "e":
                        try
                        {
                            v_explorer.Enter(System.Convert.ToInt32(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "r":
                        try
                        {
                            v_explorer.Return();
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "g":
                        try
                        {
                            System.Console.WriteLine("Arquivo recebido: " + v_explorer.Get(System.Convert.ToInt32(v_line.Split(v_separator)[1])));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "p":
                        try
                        {
                            System.Console.WriteLine("Arquivo recebido: " + v_explorer.Put(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "m":
                        try
                        {
                            System.Console.WriteLine("Diretório criado: " + v_explorer.Mkdir(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "d":
                        try
                        {
                            v_explorer.Delete(System.Convert.ToInt32(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    //case "z":
                    //    try
                    //    {
                    //        Spartacus.Utils.File v_zipfile = v_explorer.CompressDirectory("teste.zip", v_explorer.Get(System.Convert.ToInt32(v_line.Split(v_separator)[1])));
                    //    }
                    //    catch (Spartacus.Utils.Exception e)
                    //    {
                    //        System.Console.WriteLine(e.v_message);
                    //    }
                    //    break;
                    default:
                        System.Console.WriteLine("[" + v_line + "]: Comando não encontrado.");
                        break;
                }

                try
                {
                    System.Console.WriteLine(v_explorer.v_current.CompleteFileName());
                    System.Console.WriteLine();

                    if (listar)
                        v_explorer.List();

                    foreach (Spartacus.Utils.File v_file in v_explorer.v_files)
                    {
                        if (v_file.v_filetype == Spartacus.Utils.FileType.DIRECTORY)
                            System.Console.Write("D\t");
                        else
                            System.Console.Write("A\t");

                        System.Console.WriteLine("{0}\t{1}", v_file.v_id, v_file.v_name);
                    }
                    System.Console.WriteLine();
                }
                catch(Spartacus.Utils.Exception e)
                {
                    System.Console.WriteLine(e.v_message);
                }

                v_line = System.Console.ReadLine();

                listar = true;
            }
        }

        #endregion

        #region REPORT

        private static void ReportTest()
        {
            // REPORT TEST
            Spartacus.Reporting.Report v_report;

            try
            {
                v_report = new Spartacus.Reporting.Report(1, "teste3.xml");
                v_report.v_cmd.SetValue("EMID", "181");
                v_report.v_cmd.SetValue("ANO", "2013");
                v_report.Execute();
                v_report.SaveAsPDF("output.pdf");

                System.Console.WriteLine("Pronto!");
            }
            catch (Spartacus.Reporting.Exception e)
            {
                System.Console.WriteLine(e.v_message);
            }
        }

        #endregion

        #region SYNCHRONIZER

        private static void SyncTest(string[] args)
        {
            Spartacus.Utils.Synchronizer v_sync;

            v_sync = new Spartacus.Utils.Synchronizer();
            v_sync.Execute(
                Spartacus.Utils.TreeType.FROMFILE,
                args[0],
                args[1],
                Spartacus.Utils.SyncAction.CREATE,
                Spartacus.Utils.SyncAction.COPY,
                Spartacus.Utils.SyncAction.DELETE,
                Spartacus.Utils.SyncAction.DELETE,
                Spartacus.Utils.SyncAction.COPY,
                Spartacus.Utils.SyncAction.DELETE);
        }

        #endregion

        #region EXCEL

        private static void ExcelTest()
        {
            Spartacus.Utils.Excel v_excel;

            v_excel = new Spartacus.Utils.Excel();
            v_excel.Import("teste.xlsx");

            foreach (System.Data.DataTable v_table in v_excel.v_set.Tables)
            {
                System.Console.WriteLine("Planilha [{0}]:", v_table.TableName);

                foreach (System.Data.DataColumn v_column in v_table.Columns)
                    System.Console.Write("[{0}]\t", v_column.ColumnName);
                System.Console.WriteLine("");

                foreach (System.Data.DataRow v_row in v_table.Rows)
                {
                    foreach (System.Data.DataColumn v_column in v_table.Columns)
                        System.Console.Write("[{0}]\t", v_row[v_column]);
                    System.Console.WriteLine("");
                }
            }
        }

        #endregion

        #region CRYPTO

        private static void CryptoTest()
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_plaintext;
            string v_encryptedtext, v_decryptedtext;
            int k;
            System.IO.StreamWriter v_fw;
            System.IO.StreamReader v_fr;
            string v_senha;

            v_senha = "spartacus";

            v_cryptor = new Spartacus.Net.Cryptor(v_senha);

            //v_plaintext = "program=Aplicativo Teste;version=0.1.0;release=20140702;customer=Adsistem Sistemas Administrativos;cnpj=00000000000;contact=William Ivanski;licenses=5;purchase=20140801;expiration=20140930";

            //v_plaintext = "/home/william/Público/planning/Transfer/LEGISLAÇÃO/Preços de Transferência/Preços de Transferência - Legislação/comentarios a mp 478/";

            v_plaintext = "/home/william/tmp/arag/produtos.txt";

            System.Console.WriteLine("Texto puro: [{0}]\n", v_plaintext);

            v_fw = new System.IO.StreamWriter("teste.txt", false, System.Text.Encoding.UTF8);
            for (k = 0; k < 10; k++)
            {
                v_encryptedtext = v_cryptor.Encrypt(v_plaintext);
                v_decryptedtext = v_cryptor.Decrypt(v_encryptedtext);

                v_fw.WriteLine(v_encryptedtext);

                System.Console.WriteLine("Texto criptografado {0}: [{1}]\tDescriptografado: [{2}]", k, v_encryptedtext, v_decryptedtext);
            }
            v_fw.Flush();
            v_fw.Close();

            System.Console.WriteLine("");

            v_cryptor = new Spartacus.Net.Cryptor(v_senha);

            v_fr = new System.IO.StreamReader("teste.txt", System.Text.Encoding.UTF8);
            k = 0;
            while (! v_fr.EndOfStream)
            {
                v_encryptedtext = v_fr.ReadLine();

                try
                {
                    v_decryptedtext = v_cryptor.Decrypt(v_encryptedtext);

                    System.Console.WriteLine("Texto criptografado {0}: [{1}]\tDescriptografado: [{2}]", k, v_encryptedtext, v_decryptedtext);
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Senha errada!");
                }

                k++;
            }
            v_fr.Close();
        }

        private static void CryptoFileTest(string p_mode, string p_input, string p_output)
        {
            Spartacus.Net.Cryptor v_cryptor;

            v_cryptor = new Spartacus.Net.Cryptor("senha");

            if (p_mode == "E")
                v_cryptor.EncryptFile(p_input, p_output);
            else
                v_cryptor.DecryptFile(p_input, p_output);
        }

        #endregion

        #region FILEARRAY

        private static void FileArrayTest()
        {
            System.Collections.ArrayList v_list;
            Spartacus.Utils.FileArray v_filearray;

            v_list = new System.Collections.ArrayList();
            v_list.Add("/home/william/tmp/arag");
            v_list.Add("/home/william/tmp/arag2");
            v_list.Add("/home/william/tmp/arag3");

            v_filearray = new Spartacus.Utils.FileArray(v_list, "*.txt|*.TXT");

            foreach (Spartacus.Utils.File v_file in v_filearray.v_files)
                System.Console.WriteLine(v_file.CompleteFileName() + "  " + v_file.CompleteFileName(true));
        }

        #endregion
    }
}
