using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace lab3_dirichle_mvr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
		//------тестовая задача-------------------------------
		public double u_test(double x, double y)
		{
			return Math.Exp(Math.Sin(Math.PI * x * y) * Math.Sin(Math.PI * x * y));
		}
		public double f_test(double x, double y)
		{
			return 0.5 * Math.PI * Math.PI * Math.Exp(Math.Pow(Math.Sin(Math.PI * x * y), 2)) * (-4 * Math.Cos(2 * Math.PI * x * y) + Math.Cos(4 * Math.PI * x * y) - 1) * (x * x + y * y);
		}
		public double mu1_test(double y)
		{
			return 1.0;
		}
		public double mu2_test(double y)
		{
			return Math.Exp(Math.Pow(Math.Sin(2 * Math.PI * y), 2));
		}
		public double mu3_test(double x)
		{
			return 1.0;
		}
		public double mu4_test(double x)
		{
			return Math.Exp(Math.Pow(Math.Sin(Math.PI * x), 2));
		}
		//------основная задача-------------------------------
		public double f_main(double x, double y)
		{
			return -Math.Abs(x - y);
		}
		public double mu1_main(double y)
		{
			return -y * (y - 1);
		}
		public double mu2_main(double y)
		{
			return y * (1 - y);
		}
		public double mu3_main(double x)
		{
			return Math.Abs(Math.Sin(Math.PI * x));
		}
		public double mu4_main(double x)
		{
			return Math.Abs(Math.Sin(Math.PI * x)) * Math.Exp(x);
		}

		// начальное приближение
		//интерполяция по x для тестовой задачи
		public double interpolation_x_test(double j, double m, double c, double d)
		{
			double t;
			double k = (d - c) / m;
			double y = c + j * k;
			t = j / m;
			return (mu2_test(y) - mu1_test(y)) * t + mu1_test(y);
		}
		//интерполяция по y для тестовой задачи
		public double interpolation_y_test(double i, double n, double a, double b)
		{
			double t;
			double h = (b - a) / n;
			double x = a + i * h;
			t = i / n;
			return (mu4_test(x) - mu3_test(x)) * t + mu3_test(x);
		}
		//интерполяция по x для основной задачи
		public double interpolation_x_main(double j, double m, double c, double d)
		{
			double t;
			double k = (d - c) / m;
			double y = c + j * k;
			t = j / m;
			return (mu2_main(y) - mu1_main(y)) * t + mu1_main(y);
		}
		//интерполяция по y для основной задачи
		public double interpolation_y_main(double i, double n, double a, double b)
		{
			double t;
			double h = (b - a) / n;
			double x = a + i * h;
			t = i / n;
			return (mu4_main(x) - mu3_main(x)) * t + mu3_main(x);
		}
		//оптимальное значение w
		double w_opt(double h, double k)
		{
			return 2.0 - (h + k);
		}

		private void button1_Click(object sender, EventArgs e)
        {
			int n, m;           //размерность сетки
			int n_max;          //количество итераций
			double eps;         //заданная точность
			double a, b, c, d;  //границы участка
			double w;           //параметр метода

			int s = 0;                        //текущее кол-во итераций
			double eps_max = 0.0;             //достигнутая точность
			double h, k;                      //шаги по x и y
			double h2, k2, a2;                //коэффициенты метода
			double Rn = 0.0;                  //невязка на последнем шаге
			double R0 = 0.0;                  //невязка на начальном шаге
			double z_max = 0.0;               //максимальная погрешность
			double x_max = 0.0, y_max = 0.0;  //макс погрешность в узле

			double[][] v;  //массив численного решения
			double[][] u;  // массив точного решения
			double[][] f;  // массив правой части
			double[][] R;  // массив невязки

			a = 0.0;
			b = 2.0;
			c = 0.0;
			d = 1.0;

			n = Convert.ToInt32(textBox1.Text); ;
			m = Convert.ToInt32(textBox2.Text);
			eps = Convert.ToDouble(textBox3.Text);
			n_max = Convert.ToInt32(textBox4.Text);

			h = (b - a) / n;
			k = (d - c) / m;
			h2 = -(1 / (h * h));
			k2 = -(1 / (k * k));
			a2 = -2 * (h2 + k2);

			if (checkBox1.Checked)
				w = w_opt(h, k);
			else
				w = Convert.ToDouble(textBox5.Text);

			v = new double[n + 1][];
			f = new double[n + 1][];
			u = new double[n + 1][];
			R = new double[n + 1][];

			for (int i = 0; i <= n; i++)
			{
				v[i] = new double[m + 1];
				f[i] = new double[m + 1];
				u[i] = new double[m + 1];
				R[i] = new double[m + 1];
			}
			// начальное приближение
			// нулевое
			if (radioButton1.Checked)
			{
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
						v[i][j] = 0.0;
			}
			// интерполяция по x
			if (radioButton2.Checked)
			{
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						double y = c + j * k;
						double t = j / m;
						v[i][j] = interpolation_x_test(j, m, c, d);
					}
			}
			// интерполяция по y
			if (radioButton3.Checked)
			{
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						double x = a + i * h;
						double t = i / n;
						v[i][j] = interpolation_y_test(i, n, a, b);
					}
			}
			// заполнение гу
			for (int j = 0; j <= m; j++)
			{
				double y = c + j * k;
				v[0][j] = mu1_test(y);
				v[n][j] = mu2_test(y);
			}
			for (int i = 0; i <= n; i++)
			{
				double x = a + i * h;
				v[i][0] = mu3_test(x);
				v[i][m] = mu4_test(x);
			}

			// заполнение вектора правой части
			for (int j = 0; j <= m; j++)
				for (int i = 0; i <= n; i++)
				{
					double x = a + i * h;
					double y = c + j * k;
					f[i][j] = f_test(x, y);
				}

			// невязка на начальном шаге
			for (int j = 1; j < m; j++)
				for (int i = 1; i < n; i++)
					R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

			R0 = 0.0;
			for (int i = 0; i < n + 1; i++)
				for (int j = 0; j < m + 1; j++)
					R0 += R[i][j] * R[i][j];
			R0 = Math.Sqrt(R0);

			//метод верхней релаксации
			double v_old, v_new;
			double eps_curr = 0.0;
			bool flag = false;
			while (!flag)
			{
				eps_max = 0.0;
				for (int j = 1; j < m; j++)
				{
					for (int i = 1; i < n; i++)
					{
						v_old = v[i][j];
						v_new = -w * (h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1]);
						v_new = v_new + (1 - w) * a2 * v[i][j] + w * f[i][j];
						v_new = v_new / a2;
						v[i][j] = v_new;

						R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

						eps_curr = Math.Abs(v_old - v_new);
						if (eps_curr > eps_max)
							eps_max = eps_curr;
					}
				}

				s++;
				if ((eps_max <= eps) || (s >= n_max))
					flag = true;
			}
			//невязка
			Rn = 0.0;
			for (int i = 0; i < n + 1; i++)
				for (int j = 0; j < m + 1; j++)
					Rn += R[i][j] * R[i][j];
			Rn = Math.Sqrt(Rn);

			// точное решение
			for (int j = 0; j <= m; j++)
				for (int i = 0; i <= n; i++)
				{
					double x = a + i * h;
					double y = c + j * k;
					u[i][j] = u_test(x, y);
				}

			// создание и заполнение таблиц
			char[] buffer = new char[100];

			dataGridView1.Rows.Clear();
			dataGridView1.Columns.Clear();
			dataGridView1.Columns.Add("C1", " ");
			dataGridView1.Columns[0].Width = 50;
			dataGridView1.Columns[0].Frozen = true;
			dataGridView1.Columns.Add("C2", "i");
			dataGridView1.Columns[1].Width = 50;
			dataGridView1.Columns[1].Frozen = true;
			dataGridView1.Rows.Add("j", "Y\\X");  // Создание второй строки

			dataGridView2.Rows.Clear();
			dataGridView2.Columns.Clear();
			dataGridView2.Columns.Add("C2", " ");
			dataGridView2.Columns[0].Width = 50;
			dataGridView2.Columns[0].Frozen = true;
			dataGridView2.Columns.Add("C3", "i");
			dataGridView2.Columns[1].Width = 50;
			dataGridView2.Columns[1].Frozen = true;

			dataGridView3.Rows.Clear();
			dataGridView3.Columns.Clear();
			dataGridView3.Columns.Add("C4", " ");
			dataGridView3.Columns[0].Width = 50;
			dataGridView3.Columns[0].Frozen = true;
			dataGridView3.Columns.Add("C5", "i");
			dataGridView3.Columns[1].Width = 50;
			dataGridView3.Columns[1].Frozen = true;

			for (int i = 0; i <= n; i++)                        //Создание столбцов для таблиц
			{
				dataGridView1.Columns.Add(Convert.ToString(buffer), i.ToString());
				dataGridView2.Columns.Add(Convert.ToString(buffer), i.ToString());
				dataGridView3.Columns.Add(Convert.ToString(buffer), i.ToString());
			}

			dataGridView1.Rows.Add("j", "Y\\X");  // Создание второй строки
			dataGridView2.Rows.Add("j", "Y\\X");  // Создание второй строки
			dataGridView3.Rows.Add("j", "Y\\X");  // Создание второй строки

			for (int i = 0; i <= n; i++)               //Заполнение второй строки
			{
				double x = a + i * h;

				dataGridView1.Columns[i + 2].HeaderText = i.ToString();
				dataGridView2.Columns[i + 2].HeaderText = i.ToString();
				dataGridView3.Columns[i + 2].HeaderText = i.ToString();

				dataGridView1.Rows[0].Cells[i + 2].Value = x;//+2
				dataGridView2.Rows[0].Cells[i + 2].Value = x;
				dataGridView3.Rows[0].Cells[i + 2].Value = x;

			}
			for (int j = 0; j <= m; j++)          //Заполнение первых двух столбцов
			{
				dataGridView1.Rows.Add();
				dataGridView2.Rows.Add();
				dataGridView3.Rows.Add();

				for (int i = 0; i <= 1; i++)
				{
					double y = c + j * k;
					dataGridView1.Rows[j + 1].Cells[0].Value = j;
					dataGridView1.Rows[j + 1].Cells[1].Value = y;
					dataGridView2.Rows[j + 1].Cells[0].Value = j;
					dataGridView2.Rows[j + 1].Cells[1].Value = y;
					dataGridView3.Rows[j + 1].Cells[0].Value = j;
					dataGridView3.Rows[j + 1].Cells[1].Value = y;
				}
			}

			// максимальная погрешность в узле (x, y)
			for (int j = 0; j < m + 1; j++)
			{
				for (int i = 0; i < n + 1; i++)
				{
					if (Math.Abs(u[i][j] - v[i][j]) > z_max)
					{
						z_max = Math.Abs(u[i][j] - v[i][j]);
						x_max = a + i * h;
						y_max = c + j * k;
					}
				}
			}

			for (int j = 0; j <= m; j++)              //Заполнение таблиц значениями
			{
				for (int i = 0; i <= n; i++)
				{
					dataGridView1.Rows[j + 1].Cells[i + 2].Value = Math.Round(v[i][j] * 1e8) / 1e8;

					dataGridView2.Rows[j + 1].Cells[i + 2].Value = Math.Round(u[i][j] * 1e8) / 1e8;

					dataGridView3.Rows[j + 1].Cells[i + 2].Value = Math.Round(Math.Abs(u[i][j] - v[i][j]) * 1e8) / 1e8;
				}

			}

			FileStream fs = new FileStream("numeric.csv", FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			for (int j = 0; j < dataGridView1.ColumnCount; j++)
			{
				for (int i = 1; i < dataGridView1.RowCount - 1; i++)
				{
					sw.Write(Convert.ToString(dataGridView1[i, j].Value) + "\t");
				}
				sw.WriteLine();
			}
			sw.Close();
			fs.Close();
			FileStream fs1 = new FileStream("true.csv", FileMode.Create);
			StreamWriter sw1 = new StreamWriter(fs1);
			for (int j = 0; j < dataGridView2.ColumnCount; j++)
			{
				for (int i = 1; i < dataGridView2.RowCount - 1; i++)
				{
					sw1.Write(Convert.ToString(dataGridView2[i, j].Value) + "\t");
				}
				sw1.WriteLine();
			}
			sw1.Close();
			fs1.Close();
			FileStream fs2 = new FileStream("numeric_true.csv", FileMode.Create);
			StreamWriter sw2 = new StreamWriter(fs2);
			for (int j = 0; j < dataGridView3.ColumnCount; j++)
			{
				for (int i = 1; i < dataGridView3.RowCount - 1; i++)
				{
					sw2.Write(Convert.ToString(dataGridView3[i, j].Value) + "\t");
				}
				sw2.WriteLine();
			}
			sw2.Close();
			fs2.Close();

			// справка
			label6.Text = "Метод:    метод верхней релаксации";
			label7.Text = "Параметр метода: " + "   " + w.ToString();
			label8.Text = "Достигнутая точность:" + "   " + eps_max.ToString();
			label9.Text = "Количество итераций:" + "   " + s.ToString();
			//label9.Text = "Начальная невязка R0:" + "   " + (Math.Round(R0 * 1e8) / 1e8).ToString();
			label11.Text = "Невязка Rn:" + "   " + Rn.ToString();

			label10.Text = "Достигнутая погрешность: " + "   " + z_max.ToString();
			label12.Text = "Максимальная разность точного и численного решений в узле:" + "   (" + (Math.Round(x_max * 1e8) / 1e8).ToString() + ", " + (Math.Round(y_max * 1e8) / 1e8).ToString() + ")";
		}

        private void button2_Click(object sender, EventArgs e)
        {
			int n, m, n2, m2;       //размерность сетки
			int n_max;              //количество итераций
			double eps;             //заданная точность
			double a, b, c, d;      //границы участка
			double w, w2;           //параметр метода

			int s = 0, s2 = 0;                   //текущее кол-во итераций
			double eps_max = 0.0, eps_max2 = 0.0;            //достигнутая точность
			double h, k, _h, _k;                 //шаги по x и y
			double h2, k2, a2, _h2, _k2, _a2;    //коэффициенты метода
			double Rn = 0.0, Rn2 = 0.0;          //невязка
			double R0 = 0.0, R02 = 0.0;          //невязка на начальном шаге
			double z_max = 0.0;                  //максимальная погрешность
			double x_max = 0.0, y_max = 0.0;                 //макс погрешность в узле

			double[][] v;   //массив численного решения
			double[][] v2;  // массив точного решения
			double[][] f;   // массив правой части
			double[][] f2;  // массив правой части 2
			double[][] R;   // массив невязки
			double[][] R2;  // массив невязки

			a = 0.0;
			b = 2.0;
			c = 0.0;
			d = 1.0;

			n = Convert.ToInt32(textBox10.Text); ;
			m = Convert.ToInt32(textBox9.Text);
			eps = Convert.ToDouble(textBox8.Text);
			n_max = Convert.ToInt32(textBox7.Text);

			n2 = 2 * n;
			m2 = 2 * m;

			h = (b - a) / n;
			k = (d - c) / m;
			_h = (b - a) / n2;
			_k = (d - c) / m2;
			h2 = -(1 / (h * h));
			k2 = -(1 / (k * k));
			a2 = -2 * (h2 + k2);
			_h2 = -(1 / (_h * _h));
			_k2 = -(1 / (_k * _k));
			_a2 = -2 * (_h2 + _k2);

			if (checkBox2.Checked)
			{
				w = w_opt(h, k);
				w2 = w_opt(_h, _k);
			}
			else
			{
				w = Convert.ToDouble(textBox6.Text);
				w2 = w;
			}

			v = new double[n + 1][];
			f = new double[n + 1][];
			f2 = new double[n2 + 1][];
			v2 = new double[n2 + 1][];
			R = new double[n + 1][];
			R2 = new double[n2 + 1][];

			for (int i = 0; i <= n; i++)
			{
				v[i] = new double[m + 1];
				f[i] = new double[m + 1];
				R[i] = new double[m + 1];
			}
			for (int i = 0; i <= n2; i++)
			{
				f2[i] = new double[m2 + 1];
				v2[i] = new double[m2 + 1];
				R2[i] = new double[m2 + 1];
			}

			// начальное приближение
			//нулевое
			if (radioButton6.Checked)
			{
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
						v[i][j] = 0.0;
				for (int j = 1; j < m2; j++)
					for (int i = 1; i < n2; i++)
						v2[i][j] = 0.0;
			}
			//интерполяция по x
			if (radioButton5.Checked)
			{
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						double y = c + j * k;
						double t = j / m;
						v[i][j] = interpolation_x_main(j, m, c, d);
					}
				for (int j = 1; j < m2; j++)
					for (int i = 1; i < n2; i++)
					{
						double y = c + j * _k;
						double t = j / m2;
						v2[i][j] = interpolation_x_main(j, m2, c, d);
					}
			}
			//интерполяция по y
			if (radioButton4.Checked)
			{
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						double x = a + i * h;
						double t = i / n;
						v[i][j] = interpolation_y_main(i, n, a, b);
					}
				for (int j = 1; j < m2; j++)
					for (int i = 1; i < n2; i++)
					{
						double x = a + i * _h;
						double t = i / n2;
						v2[i][j] = interpolation_y_main(i, n2, a, b);
					}
			}
			// заполнение гу
			for (int j = 0; j <= m; j++)
			{
				double y = c + j * k;
				v[0][j] = mu1_main(y);
				v[n][j] = mu2_main(y);
			}
			for (int i = 0; i <= n; i++)
			{
				double x = a + i * h;
				v[i][0] = mu3_main(x);
				v[i][m] = mu4_main(x);
			}
			for (int j = 0; j <= m2; j++)
			{
				double y = c + j * _k;
				v2[0][j] = mu1_main(y);
				v2[n2][j] = mu2_main(y);
			}
			for (int i = 0; i <= n2; i++)
			{
				double x = a + i * _h;
				v2[i][0] = mu3_main(x);
				v2[i][m2] = mu4_main(x);
			}

			// заполнение правой части
			for (int j = 0; j <= m; j++)
				for (int i = 0; i <= n; i++)
				{
					double x = a + i * h;
					double y = c + j * k;
					f[i][j] = f_main(x, y);
				}
			for (int j = 0; j <= m2; j++)
				for (int i = 0; i <= n2; i++)
				{
					double x = a + i * _h;
					double y = c + j * _k;
					f2[i][j] = f_main(x, y);
				}

			// невязка на начальном шаге
			for (int j = 1; j < m; j++)
				for (int i = 1; i < n; i++)
					R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

			R0 = 0.0;
			for (int i = 0; i < n + 1; i++)
				for (int j = 0; j < m + 1; j++)
					R0 += R[i][j] * R[i][j];
			R0 = Math.Sqrt(R0);

			for (int j = 1; j < m2; j++)
				for (int i = 1; i < n2; i++)
					R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];

			R02 = 0.0;
			for (int i = 0; i < n2 + 1; i++)
				for (int j = 0; j < m2 + 1; j++)
					R02 += R2[i][j] * R2[i][j];
			R02 = Math.Sqrt(R02);

			// метод верхней релаксации  для v(x,y)
			double v_old, v_new;
			double eps_curr = 0.0;
			bool flag = false;
			while (!flag)
			{
				eps_max = 0.0;
				for (int j = 1; j < m; j++)
				{
					for (int i = 1; i < n; i++)
					{
						v_old = v[i][j];
						v_new = -w * (h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1]);
						v_new = v_new + (1 - w) * a2 * v[i][j] + w * f[i][j];
						v_new = v_new / a2;
						v[i][j] = v_new;

						R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

						eps_curr = Math.Abs(v_old - v_new);
						if (eps_curr > eps_max)
							eps_max = eps_curr;
					}
				}

				s++;
				if ((eps_max <= eps) || (s >= n_max))
					flag = true;
			}
			//невязка для v(x,y)
			Rn = 0.0;
			for (int i = 0; i < n + 1; i++)
				for (int j = 0; j < m + 1; j++)
					Rn += R[i][j] * R[i][j];
			Rn = Math.Sqrt(Rn);

			// метод верхней релаксации  для v2(x,y)
			double v_old2, v_new2;
			double eps_curr2 = 0.0;
			bool flag2 = false;
			while (!flag2)
			{
				eps_max2 = 0.0;
				for (int j = 1; j < m2; j++)
				{
					for (int i = 1; i < n2; i++)
					{
						v_old2 = v2[i][j];
						v_new2 = -w2 * (_h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1]);
						v_new2 = v_new2 + (1 - w2) * _a2 * v2[i][j] + w2 * f2[i][j];
						v_new2 = v_new2 / _a2;
						v2[i][j] = v_new2;

						R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];

						eps_curr2 = Math.Abs(v_old2 - v_new2);
						if (eps_curr2 > eps_max2)
							eps_max2 = eps_curr2;
					}
				}

				s2++;
				if ((eps_max2 <= eps) || (s2 >= n_max))
					flag2 = true;
			}
			//невязка для v2(x,y)
			Rn2 = 0.0;
			for (int i = 0; i < n2 + 1; i++)
				for (int j = 0; j < m2 + 1; j++)
					Rn2 += R2[i][j] * R2[i][j];
			Rn2 = Math.Sqrt(Rn2);

			char[] buffer = new char[100];
			dataGridView4.Rows.Clear();
			dataGridView4.Columns.Clear();

			dataGridView5.Rows.Clear();
			dataGridView5.Columns.Clear();

			dataGridView6.Rows.Clear();
			dataGridView6.Columns.Clear();

			dataGridView4.Columns.Add("C1", "");
			dataGridView4.Columns[0].Width = 50;
			dataGridView4.Columns[0].Frozen = true;
			dataGridView4.Columns.Add("C2", "i");
			dataGridView4.Columns[1].Width = 50;
			dataGridView4.Columns[1].Frozen = true;

			dataGridView5.Columns.Add("C1", "");
			dataGridView5.Columns[0].Width = 50;
			dataGridView5.Columns[0].Frozen = true;
			dataGridView5.Columns.Add("C2", "i");
			dataGridView5.Columns[1].Width = 50;
			dataGridView5.Columns[1].Frozen = true;

			dataGridView6.Columns.Add("C1", "");
			dataGridView6.Columns[0].Width = 50;
			dataGridView6.Columns[0].Frozen = true;
			dataGridView6.Columns.Add("C2", "i");
			dataGridView6.Columns[1].Width = 50;
			dataGridView6.Columns[1].Frozen = true;

			for (int i = 0; i <= n; i++)                        //Создание столбцов для таблиц
			{

				dataGridView4.Columns.Add(Convert.ToString(buffer), i.ToString());
				dataGridView5.Columns.Add(Convert.ToString(buffer), i.ToString());
				dataGridView6.Columns.Add(Convert.ToString(buffer), i.ToString());
			}

			dataGridView4.Rows.Add("j", "Y\\X");  // Создание второй строки
			dataGridView5.Rows.Add("j", "Y\\X");  // Создание второй строки
			dataGridView6.Rows.Add("j", "Y\\X");  // Создание второй строки


			for (int i = 0; i <= n; i++)               //Заполнение второй строки
			{
				double x = a + i * h;

				dataGridView4.Columns[i + 2].HeaderText = i.ToString();
				dataGridView5.Columns[i + 2].HeaderText = i.ToString();
				dataGridView6.Columns[i + 2].HeaderText = i.ToString();
				dataGridView4.Rows[0].Cells[i + 2].Value = x;
				dataGridView5.Rows[0].Cells[i + 2].Value = x;
				dataGridView6.Rows[0].Cells[i + 2].Value = x;

			}
			for (int j = 0; j <= m; j++)          //Заполнение первых двух столбцов
			{
				double y = a + j * k;
				dataGridView4.Rows.Add();
				dataGridView5.Rows.Add();
				dataGridView6.Rows.Add();
				for (int i = 0; i <= 1; i++)
				{
					dataGridView4.Rows[j + 1].Cells[0].Value = j;
					dataGridView4.Rows[j + 1].Cells[1].Value = y;
					dataGridView5.Rows[j + 1].Cells[0].Value = j;
					dataGridView5.Rows[j + 1].Cells[1].Value = y;
					dataGridView6.Rows[j + 1].Cells[0].Value = j;
					dataGridView6.Rows[j + 1].Cells[1].Value = y;
				}
			}
			for (int j = 0; j < m + 1; j++)
			{
				for (int i = 0; i < n + 1; i++)
				{
					if (Math.Abs(v[i][j] - v2[2 * i][2 * j]) > z_max)
					{
						z_max = Math.Abs(v[i][j] - v2[2 * i][2 * j]);
						x_max = a + i * h;
						y_max = c + j * k;
					}
				}
			}
			for (int j = 0; j <= m; j++)              //Заполнение таблиц значениями
			{
				for (int i = 0; i <= n; i++)
				{
					dataGridView4.Rows[j + 1].Cells[i + 2].Value = Math.Round(v[i][j] * 1e8) / 1e8;

					dataGridView5.Rows[j + 1].Cells[i + 2].Value = Math.Round(v2[2 * i][2 * j] * 1e8) / 1e8;

					dataGridView6.Rows[j + 1].Cells[i + 2].Value = Math.Round(Math.Abs(v[i][j] - v2[2 * i][2 * j]) * 1e8) / 1e8;
				}
			}
			FileStream fs = new FileStream("numeric.csv", FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			for (int j = 0; j < dataGridView4.ColumnCount; j++)
			{
				for (int i = 1; i < dataGridView4.RowCount - 1; i++)
				{
					sw.Write(Convert.ToString(dataGridView4[i, j].Value) + "\t");
				}
				sw.WriteLine();
			}
			sw.Close();
			fs.Close();
			FileStream fs1 = new FileStream("numeric2.csv", FileMode.Create);
			StreamWriter sw1 = new StreamWriter(fs1);
			for (int j = 0; j < dataGridView5.ColumnCount; j++)
			{
				for (int i = 1; i < dataGridView5.RowCount - 1; i++)
				{
					sw1.Write(Convert.ToString(dataGridView5[i, j].Value) + "\t");
				}
				sw1.WriteLine();
			}
			sw1.Close();
			fs1.Close();
			FileStream fs2 = new FileStream("numeric_numeric2.csv", FileMode.Create);
			StreamWriter sw2 = new StreamWriter(fs2);
			for (int j = 0; j < dataGridView6.ColumnCount; j++)
			{
				for (int i = 1; i < dataGridView6.RowCount - 1; i++)
				{
					sw2.Write(Convert.ToString(dataGridView6[i, j].Value) + "\t");
				}
				sw2.WriteLine();
			}
			sw2.Close();
			fs2.Close();

			// справка
			label15.Text = "Метод:     метод верхней релаксации";
			label13.Text = "Метод:     метод верхней релаксации";
			label16.Text = "Параметр метода: " + "   " + w.ToString();
			label14.Text = "Параметр метода: " + "   " + w2.ToString();
			label19.Text = "Достигнутая точность:" + "   " + eps_max.ToString();
			label28.Text = "Достигнутая точность:" + "   " + eps_max2.ToString();
			label18.Text = "Количество итераций:" + "   " + s.ToString();
			label30.Text = "Количество итераций:" + "   " + s2.ToString();
			//label9.Text = "Начальная невязка R0:" + "   " + (Math.Round(R0 * 1e8) / 1e8).ToString();
			label17.Text = "Невязка Rn:" + "   " + Rn.ToString();
			label27.Text = "Невязка Rn:" + "   " + Rn2.ToString();

			label26.Text = "Достигнутая точность решения: " + "   " + z_max.ToString();
			label25.Text = "Максимальная разность точного и численного решений в узле:" + "   (" + (Math.Round(x_max * 1e8) / 1e8).ToString() + ", " + (Math.Round(y_max * 1e8) / 1e8).ToString() + ")";

			}
    }
}
