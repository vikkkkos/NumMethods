using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab2_spline
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public double[] A, B, C, D;     // коэффициенты сплайна
        public double[] X;              // узлы сетки
        public int task = 1;            // выбор задачи ( по умолчанию - тестовая)
        public int n;                   // число разбиений
        public double a = -1, b = 1;    //границы отрезка
        public double mu1 = 0, mu2 = 0; // граничные условия
        public double h;                // шаг сетки

        public double function(double x)
        { 
            if (task == 1)
            {
                if ((x >= -1.0) && (x <= 0.0))
                {
                    return (Math.Pow(x, 3.0) + 3.0 * Math.Pow(x, 2.0));
                }
                else
                {
                    return (-Math.Pow(x, 3.0) + 3.0 * Math.Pow(x, 2.0));
                }
            }
            if (task == 2)
            {
                return Math.Sqrt(1 + Math.Pow(x, 4.0));
            }
            if (task == 3)
            {
                return Math.Sqrt(1 + Math.Pow(x, 4.0)) + Math.Cos(10 * x);
            }
            return (0.0);
        }

        public double function_first_derivative(double x)
        {
            if (task == 1)
            {
                if ((x >= -1.0) && (x <= 0.0))
                {
                    return (3.0 * Math.Pow(x, 2.0) + 6.0 * x);
                }
                else
                {
                    return (-3.0 * Math.Pow(x, 2.0) + 6.0 * x);
                }
            }
            if (task == 2)
            {
                return (2 * Math.Pow(x, 3.0)) / Math.Sqrt(1 + Math.Pow(x, 4.0));
            }
            if (task == 3)
            {
                return (2 * Math.Pow(x, 3.0)) / Math.Sqrt(1 + Math.Pow(x, 4.0)) - 10 * Math.Sin(10 * x);
            }
            return 0.0;
        }

        public double function_second_derivative(double x)
        { 
            if (task == 1)
            {
                if ((x >= -1.0) && (x <= 0.0))
                {
                    return (6.0 * x + 6.0);
                }
                else
                {
                    return (-6.0 * x + 6.0);
                }
            }
            if (task == 2)
            {
                return (Math.Sqrt(1 + Math.Pow(x, 4.0)) * (2 * Math.Pow(x, 6.0) + 6 * x * x)) / (Math.Pow(x, 8.0) + 2 * Math.Pow(x, 4.0) + 1);
            }
            if (task == 3)
            {
                return (Math.Sqrt(1 + Math.Pow(x, 4.0)) * (2 * Math.Pow(x, 6.0) + 6 * x * x)) / (Math.Pow(x, 8.0) + 2 * Math.Pow(x, 4.0) + 1) - 100 * Math.Cos(10 * x);
            }
            return 0.0;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            task = 1;
            a = -1;
            b = 1;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            task = 2;
            a = 0;
            b = 1;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            mu1 = 0;
            mu2 = 0;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (task == 1)
            {
                mu1 = 0;
                mu2 = 0;
            }
            if (task == 2)
            {
                mu1 = 0;
                mu2 = (Math.Sqrt(2) * (8)) / (4);
            }
            if (task == 3)
            {
                mu1 = -100 * Math.Cos(10 * 0);
                mu2 = (Math.Sqrt(2) * (8)) / (4) - 100 * Math.Cos(10);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            task = 3;
            a = 0;
            b = 1;
        }

        public double spline(double x)
        {
            for (int i = 1; i <= n; i++)
            {
                if ((x >= X[i - 1]) && (x <= X[i]))
                {
                    return (A[i] + B[i] * (x - X[i]) + C[i] / 2 * Math.Pow((x - X[i]), 2) + D[i] / 6 * Math.Pow((x - X[i]), 3));
                }
            }
            return (0.0);
        }

        public double spline_first_derivative(double x)
        {
            for (int i = 1; i <= n; i++)
            {
                if ((x >= X[i - 1]) && (x <= X[i]))
                {
                    return (B[i] + C[i] * (x - X[i]) + D[i] / 2 * Math.Pow((x - X[i]), 2));
                }
            }
            return (0.0);
        }

        public double spline_second_derivative(double x)
        {
            for (int i = 1; i <= n; i++)
            {
                if ((x >= X[i - 1]) && (x <= X[i]))
                {
                    return (C[i] + D[i] * (x - X[i]));
                }
            }
            return (0.0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var series in chart1.Series)
                series.Points.Clear();
            foreach (var series in chart2.Series)
                series.Points.Clear();
            foreach (var series in chart3.Series)
                series.Points.Clear();
            n = System.Convert.ToInt32(textBox1.Text);

            h = (b - a) / n;
            A = new double[n + 1];
            B = new double[n + 1];
            C = new double[n + 1];
            D = new double[n + 1];
            X = new double[n + 1];

            // нахождение коэффициентов
            // A
            for (int i = 0; i <= n; i++)
            {
                X[i] = a + i * h; 
                A[i] = function(X[i]);	
            }
            // C
            double[] Alpha = new double[n + 1];
            double[] Beta = new double[n + 1];
            Alpha[1] = 0;
            Beta[1] = mu1;
            for (int i = 1; i <= n - 1; i++)
            {
                Alpha[i + 1] = (-1.0) * h / (Alpha[i] * h + 4 * h);
                Beta[i + 1] = ((-6.0 / h) * (A[i + 1] - 2 * A[i] + A[i - 1]) + Beta[i] * h) / (-4 * h - Alpha[i] * h);
            }
            C[n] = mu2;
            for (int i = n; i >= 1; i--)
            {
                C[i - 1] = Alpha[i] * C[i] + Beta[i];
            }
            // B, D
            for (int i = 1; i <= n; i++)
            {
                B[i] = (A[i] - A[i - 1]) / h + h * (2 * C[i] + C[i - 1]) / 6;
                D[i] = (C[i] - C[i - 1]) / h;
            }
            double max_f_s = 0;
            double max_df_ds = 0;
            double max_d2f_d2s = 0;
            double max_x = 0;
            double max_dx = 0;
            double max_d2x = 0;
            double Tmp;

            for (double x = a; x <= b; x += h / 2.0)
            {
                Tmp = Math.Abs(function(x) - spline(x));
                if (Tmp > max_f_s)
                {
                    max_f_s = Tmp;
                    max_x = x;
                }
                Tmp = Math.Abs(function_first_derivative(x) - spline_first_derivative(x));
                if (Tmp > max_df_ds)
                {
                    max_df_ds = Tmp;
                    max_dx = x;
                }
                Tmp = Math.Abs(function_second_derivative(x) - spline_second_derivative(x));
                if (Tmp > max_d2f_d2s)
                {
                    max_d2f_d2s = Tmp;
                    max_d2x = x;
                }
            }
            Tmp = Math.Abs(function(b) - spline(b));
            if (Tmp > max_f_s)
            {
                max_f_s = Tmp;
                max_x = b;
            }
            Tmp = Math.Abs(function_first_derivative(b) - spline_first_derivative(b));
            if (Tmp > max_df_ds)
            {
                max_df_ds = Tmp;
                max_dx = b;
            }
            Tmp = Math.Abs(function_second_derivative(b) - spline_second_derivative(b));
            if (Tmp > max_d2f_d2s)
            {
                max_d2f_d2s = Tmp;
                max_d2x = b;
            }
            dataGridView1.Rows.Clear();
            for (int i = 1; i <= n; i++)
            {
                dataGridView1.Rows.Add(i, X[i - 1], X[i], A[i], B[i], C[i], D[i]);
            }
            dataGridView2.Rows.Clear();
            double y = a;
            for (int i = 1; i <= 2*n; i++)
            {
                dataGridView2.Rows.Add(i, y, function(y), spline(y), Math.Abs(function(y) - spline(y)), function_first_derivative(y), spline_first_derivative(y), 
                    Math.Abs(function_first_derivative(y) - spline_first_derivative(y)), function_second_derivative(y), spline_second_derivative(y), Math.Abs(function_second_derivative(y) - spline_second_derivative(y)));
                y += h / 2.0;
            }
            dataGridView2.Rows.Add(2*n+1, b, function(b), spline(b), Math.Abs(function(b) - spline(b)), function_first_derivative(b), spline_first_derivative(b),
                    Math.Abs(function_first_derivative(b) - spline_first_derivative(b)), function_second_derivative(b), spline_second_derivative(b), Math.Abs(function_second_derivative(b) - spline_second_derivative(b)));
            y = a;
            for (int i = 0; i < 2*n; i++)
            {
                chart1.Series[0].Points.AddXY(y, function(y));
                chart1.Series[1].Points.AddXY(y, spline(y));
                chart1.Series[2].Points.AddXY(y, function(y) - spline(y));
                chart2.Series[0].Points.AddXY(y, function_first_derivative(y));
                chart2.Series[1].Points.AddXY(y, spline_first_derivative(y));
                chart2.Series[2].Points.AddXY(y, function_first_derivative(y) - spline_first_derivative(y));
                chart3.Series[0].Points.AddXY(y, function_second_derivative(y));
                chart3.Series[1].Points.AddXY(y, spline_second_derivative(y));
                chart3.Series[2].Points.AddXY(y, function_second_derivative(y) - spline_second_derivative(y));
                y += h / 2.0;
            }
            chart1.Series[0].Points.AddXY(b, function(b));
            chart1.Series[1].Points.AddXY(b, spline(b));
            chart1.Series[2].Points.AddXY(b, function(b) - spline(b));
            chart2.Series[0].Points.AddXY(b, function_first_derivative(b));
            chart2.Series[1].Points.AddXY(b, spline_first_derivative(b));
            chart2.Series[2].Points.AddXY(b, function_first_derivative(b) - spline_first_derivative(b));
            chart3.Series[0].Points.AddXY(b, function_second_derivative(b));
            chart3.Series[1].Points.AddXY(b, spline_second_derivative(b));
            chart3.Series[2].Points.AddXY(b, function_second_derivative(b) - spline_second_derivative(b));
            labelSa.Text = "S\"(a) = " + mu1.ToString();
            labelSb.Text = "S\"(b) = " + mu2.ToString();
            label2.Text = "Сетка сплайна: n=" + n.ToString();
            label3.Text = "Контрольная сетка: N=" + (2 * n).ToString();
            label4.Text = "Max|f(x)-S(x)|=" + max_f_s.ToString() + ", \nпри x=" + max_x.ToString();
            label5.Text = "Max|f'(x)-S'(x)|=" + max_df_ds.ToString() + ", \nпри x=" + max_dx.ToString();
            label6.Text = "Max|f''(x)-S''(x)|=" + max_d2f_d2s.ToString() + ", \nпри x=" + max_d2x.ToString();
        }
    }
}
