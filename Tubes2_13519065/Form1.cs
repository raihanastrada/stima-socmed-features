﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Connect
{


    public partial class Form1 : Form
    {
        Microsoft.Msagl.Drawing.Graph graph; // The graph that MSAGL accepts
        Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer(); // Graph viewer engine
        Graph Graf;
        int JumlahNode;
        bool graphLoaded = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button_LoadFile_Click(object sender, EventArgs e)
        {
            // Browse Document
            openFileGraph.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileGraph.InitialDirectory = Directory.GetCurrentDirectory();

            // Show file dialog
            DialogResult result = openFileGraph.ShowDialog();

            if (result == DialogResult.OK)
            {
                graph = new Microsoft.Msagl.Drawing.Graph("graph"); // Initialize new MSAGL graph                
                // Read input file
                using (StreamReader bacafile = new StreamReader(openFileGraph.OpenFile()))
                {
                    string baca = bacafile.ReadLine();
                    if (baca == null || baca == "0")
                    {
                        MessageBox.Show("File Kosong");
                    }
                    else
                    {
                        graphLoaded = true;
                        JumlahNode = Int32.Parse(baca);
                        Graf = new Graph(JumlahNode);

                        while (bacafile.Peek() >= 0)
                        {
                            baca = bacafile.ReadLine(); // Read file line by line
                            string[] cur_line = baca.Split(' ');
                            graph.AddEdge(cur_line[0], cur_line[1]);
                            graph.AddEdge(cur_line[1], cur_line[0]);
                        }

                        try
                        {
                            DrawGraph();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Input Text Mengandung Graf Siklik");
                        }
                    }
                }
            }
        }

        private void DrawGraph()
        {
            // Bind graph to viewer engine
            viewer.Graph = graph;
            // Bind viewer engine to the panel
            panel_DrawGraph.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            panel_DrawGraph.Controls.Add(viewer);
            panel_DrawGraph.ResumeLayout();
        }

        //Graph declaration
        class Graph
        {
            private List<int>[] sisi;
            private int[] depth;
            private int simpul;

            public Graph(int n)
            {
                simpul = n;
                sisi = new List<int>[n + 1];
                depth = new int[n + 1];
                for (int i = 0; i <= n; i++)
                {
                    sisi[i] = new List<int>();
                    depth[i] = 0;
                }
            }
            ~Graph()
            {
                for (int i = 0; i <= simpul; i++)
                {
                    sisi[i] = null;
                }
                sisi = null;
                depth = null;
            }
        }

    }
}