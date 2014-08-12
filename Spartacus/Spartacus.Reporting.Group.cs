using System;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Group.
    /// Representa um agrupamento de dados do relatório.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Nível do grupo.
        /// </summary>
        public int v_level;

        /// <summary>
        /// Coluna associada ao grupo.
        /// </summary>
        public string v_column;

        /// <summary>
        /// Ordenação do grupo.
        /// </summary>
        public string v_sort;

        /// <summary>
        /// Indica se o cabeçalho do grupo deve ser mostrado ou não.
        /// </summary>
        public bool v_showheader;

        /// <summary>
        /// Indica se o rodapé do grupo deve ser mostrado ou não.
        /// </summary>
        public bool v_showfooter;

        /// <summary>
        /// Lista de campos do cabeçalho.
        /// </summary>
        public System.Collections.ArrayList v_headerfields;

        /// <summary>
        /// Lista de campos do rodapé.
        /// </summary>
        public System.Collections.ArrayList v_footerfields;

        /// <summary>
        /// Tabela com os dados do grupo.
        /// </summary>
        public System.Data.DataTable v_table;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Group"/>.
        /// </summary>
        public Group()
        {
            this.v_showheader = true;
            this.v_showfooter = true;

            this.v_headerfields = new System.Collections.ArrayList();
            this.v_footerfields = new System.Collections.ArrayList();
        }

        /// <summary>
        /// Constrói dados do grupo.
        /// Percorre tabela de dados do relatório, filtrando e distinguindo os dados pela coluna do grupo.
        /// </summary>
        /// <param name="p_table">Tabela de dados do relatório.</param>
        public void Build(System.Data.DataTable p_table)
        {
            System.Collections.ArrayList v_allcolumns_temp;
            string[] v_allcolumns;
            int k;

            // alocando lista de colunas
            v_allcolumns_temp = new System.Collections.ArrayList();

            // adicionando coluna do grupo
            v_allcolumns_temp.Add(this.v_column);

            // adicionando todas as colunas do cabeçalho do grupo
            for (k = 0; k < this.v_headerfields.Count; k++)
                v_allcolumns_temp.Add(((Spartacus.Reporting.Field)this.v_headerfields[k]).v_column);

            // adicionando todas as colunas do rodapé do grupo
            for (k = 0; k < this.v_footerfields.Count; k++)
            {
                if (! v_allcolumns_temp.Contains(((Spartacus.Reporting.Field)this.v_footerfields [k]).v_column))
                    v_allcolumns_temp.Add(((Spartacus.Reporting.Field)this.v_footerfields [k]).v_column);
            }

            // alocando vetor de string
            v_allcolumns = new string[v_allcolumns_temp.Count];

            // copiando nomes de colunas para o vetor de string
            for (k = 0; k < v_allcolumns_temp.Count; k++)
                v_allcolumns [k] = (string) v_allcolumns_temp[k];

            // filtrando dados distintos pela lista de colunas, e armazenando em tabela
            this.v_table = p_table.DefaultView.ToTable(true, v_allcolumns);
        }
    }
}
