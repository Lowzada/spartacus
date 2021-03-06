![Spartacus Icon](https://raw.githubusercontent.com/wind39/spartacus/master/Spartacus/icon/spartacus_128x128.png)

Spartacus is a business library written in C#, focused on:
  - Access many types of databases in a fast, easy to use and generic way;
  - Render reports from a unique XML markup to PDF, using own database objects;
  - Really fast import and export spreadsheets (CSV, DBF and XLSX), with no need of Excel or any other piece of software;
  - Easy to work with cryptography of files and messages.

NuGet package: https://www.nuget.org/packages/Spartacus

Examples of how to use Spartacus are here: https://github.com/wind39/spartacus/wiki

Discussion about Spartacus is here: https://www.reddit.com/r/spartacuslibrary/

Spartacus contains this managed libraries:
  - Mono.Data.Sqlite (http://www.mono-project.com/docs/database-access/providers/sqlite/)
  - Mono.Data.Tds (http://www.mono-project.com/docs/database-access/providers/sqlclient/)
  - Oracle ODP.NET, Managed Driver (https://www.nuget.org/packages/Oracle.ManagedDataAccess/)
  - Firebird .NET Provider (http://www.firebirdsql.org/en/net-provider/)
  - Npgsql (https://github.com/npgsql/npgsql)
  - MySQL Connector/NET (http://dev.mysql.com/downloads/connector/net/1.0.html)
  - System.Data.SqlServerCe (https://www.nuget.org/packages/Microsoft.SqlServer.Compact/)
  - EPPlus (http://epplus.codeplex.com/)
  
Spartacus reuse code from this C# projects:
  - Mono open source implementation of Microsoft's .NET Framework (https://github.com/mono/mono)
  - PDFjet Open Source Edition (http://pdfjet.com/os/edition.html)
  - SejExcel, by Jose Segarra (https://github.com/jsegarra1971/SejExcelExport)
  - ZipStorer, by Jaime Olivares (https://github.com/jaime-olivares/zipstorer)
  - FastDBF, by Ahmed Lacevic (https://github.com/SocialExplorer/FastDBF)
  - MultiColumnCombobox, by Nish Nishant (http://www.codeproject.com/Articles/19781/A-data-bound-multi-column-combobox)

Spartacus optionally use this libraries:
  - SQLite (https://www.sqlite.org/)
  - UCanAccess (http://ucanaccess.sourceforge.net/site.html)
  - IKVM.NET (http://www.ikvm.net/)
  - Native MS SQL Server CE (https://www.nuget.org/packages/Microsoft.SqlServer.Compact/)
  - Firebird Embedded (http://www.firebirdsql.org/manual/ufb-cs-embedded.html)

Spartacus DLL is built with Mono C# Compiler (http://www.mono-project.com/docs/about-mono/languages/csharp/), merged with ILRepack (https://github.com/gluck/il-repack) and packaged with NuGet (https://www.nuget.org/).

Spartacus is successfully used by this projects:
  - OmniDB (http://www.omnidb.com.br)

I also would like to thank the following people and companies for help testing and suggesting improvements:
  - Planning Service Transfer Pricing (http://www.planningservice.com.br)
  - Adsistem Sistemas Administrativos (http://www.adsistem.com.br)
  - Rafael Thofehrn Castro (http://www.rafaelthca.com.br)
  - Cristiano Raffi Cunha (http://cristianoprogramador.com)