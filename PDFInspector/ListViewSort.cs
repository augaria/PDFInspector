using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDFInspector
{
    public class ListViewSort : IComparer
    {
        private int col;
        private bool descK;


        public ListViewSort()
        {
            col = 0;
        }
        public ListViewSort(int column, object Desc)
        {
            descK = (bool)Desc;
            col = column; 
        }
        public int Compare(object x, object y)
        { 


            int res =string.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);

            if (descK)
            {
                return -res;
            }
            else
            {
                return res;
            }
        }
    }
}
