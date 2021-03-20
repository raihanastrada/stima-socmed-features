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
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Connect
{

    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern System.IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        Microsoft.Msagl.Drawing.Graph graph; // The graph that MSAGL accepts
        Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        // Graph viewer engine
        private Graph graf;
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
                        this.graf = new Graph(bacafile);
                        comboBox1.Items.Clear();
                        comboBox2.Items.Clear();
                        foreach (KeyValuePair<string, List<string>> entry1 in graf.getAdjacent())
                        {
                            comboBox1.Items.Add(entry1.Key);
                            comboBox2.Items.Add(entry1.Key);
                        }
                            DrawGraph(this.graf);
                    }
                }
            }
        }

        private void DrawGraph(Graph graf)
        {

            List<Tuple<string, string>> visited = new List<Tuple<string, string>>();
            graph = new Microsoft.Msagl.Drawing.Graph("graph"); // Initialize new MSAGL graph                

            foreach (KeyValuePair<string, List<string>> entry1 in graf.getAdjacent())
            {
                // do something with entry.Value or entry.Key
                foreach (var entry2 in entry1.Value)
                {
                    if (!visited.Contains(Tuple.Create(entry1.Key, entry2)) && !visited.Contains(Tuple.Create(entry2, entry1.Key)))
                    {
                        graph.AddEdge(entry1.Key,entry2).Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                    }
                    visited.Add(Tuple.Create(entry1.Key, entry2));
                }
            }
            drawContainer(graph);
        }
        public void drawContainer(Microsoft.Msagl.Drawing.Graph graph)
        {
            graph.LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings();
            viewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            viewer.Graph = graph;
            // Bind graph to viewer engine
            viewer.Graph = graph;
            // Bind viewer engine to the panel
            panel_DrawGraph.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            panel_DrawGraph.Controls.Add(viewer);
            panel_DrawGraph.ResumeLayout();
        }
        private void panel_DrawGraph_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Bikin fungsi dfs bfs, trs panggil disini. Ini button submit ceunah wkwk 
            if (radioButton1.Checked)
            {
                if (comboBox3.Text == "Show Graph")
                {
                    string x = "DFS1";
                    textBox1.Text = x;
                }
                else if (comboBox3.Text == "Friend Recommendation")
                {
                    string x = "DFS2";
                    textBox1.Text = x;
                }
                else if (comboBox3.Text == "Explore Friends")
                {
                    string x = "DFS3";
                    textBox1.Text = x;
                }

            }
            if (radioButton2.Checked)
            {
                if (comboBox3.Text == "Show Graph")
                {
                    string x = "BFS1";
                    textBox1.Text = x;
                }
                else if (comboBox3.Text == "Friend Recommendation")
                {   
                    // Siapin parameter yang mau digunain buat BFS
                    List<List<string>> queue = new List<List<string>>();   
                    List<List<string>> solution = new List<List<string>>(); // Mencatat solusi rute
                    Dictionary<string, List<string>> adjacent = new Dictionary<string, List<string>>();
                    List<string> route = new List<string>();
                    string accName = comboBox3.Text;    // Ambil simpul awal
                    route.Add(accName);
                    queue.Add(route);      // Masukin simpul awal ke queue
                    adjacent = graf.getAdjacent();

                    // Mencari rekomendasi teman dengan BFS
                    Graph.BFS b;
                    b= new Graph.BFS(this.graf, ref graph, ref panel_DrawGraph, ref viewer);
                    b.friendsRecommendation(accName, adjacent, queue, solution);

                    // Tampilin di notes
                    string x = "Daftar rekomendasi teman untuk akun "+accName+":";
                    textBox1.Text = x;
                    x = (queue.ElementAt(0)).Last();
                    textBox1.Text = x;
                    x = "Test Rute: ";
                    textBox1.Text = x;
                    x = queue.ElementAt(0).ToString();
                }
                else if (comboBox3.Text == "Explore Friends")
                {
                    Graph.BFS b;
                    b = new Graph.BFS(this.graf, ref graph, ref panel_DrawGraph, ref viewer);
                    List<string> answer = new List<string>(b.exploreFriends(comboBox1.Text, comboBox2.Text));
                    string x = "Nama akun : " + comboBox1.Text + " dan " + comboBox2.Text + "\r\n";
                    if (answer==null) {
                        x = x + "Tidak ada jalur koneksi yang tersedia.\r\n";
                        x = x + "Anda harus membangun jalur itu sendiri.\r\n";
                    }
                    else
                    {
                        x = x + String.Join(" → ", answer)+ "\r\n";
                        if (answer.Count == 1)
                        {
                            x = x + "Its node ";
                        }
                        else if (answer.Count == 2)
                        {
                            x = x + "mutual friend ";
                        }
                        else if (answer.Count == 3)
                        {
                            x = x + (answer.Count - 2).ToString() + "st-degree ";
                        }
                        else if (answer.Count == 4)
                        {
                            x = x + (answer.Count - 2).ToString() + "nd-degree ";
                        }
                        else if (answer.Count == 5)
                        {
                            x = x + (answer.Count - 2).ToString() + "rd-degree ";
                        }
                        else
                        {
                            x = x + (answer.Count - 2).ToString() + "th-degree ";
                        }
                        x = x + "connection.\r\n";
                    }
                    textBox1.Text = x;
                }
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // kalo mau disable
            //button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button1.Width, button2.Height, 30, 30));
            button2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button2.Width, button2.Height, 30, 30));
            button3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button3.Width, button3.Height, 30, 30));
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}