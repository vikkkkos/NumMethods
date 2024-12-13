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

namespace lab4_dirichle_iter
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

		private void button1_Click(object sender, EventArgs e)
        {
			int n, m;               //размерность сетки
			int n_max;              //количество итераций
			double eps;             //заданная точность
			double a, b, c, d;      //границы участка
			int K;                  //количество параметров чебышева
			double tau;             //параметр мпи

			int s = 0;                        //текущее кол-во итераций
			double eps_max = 0.0;             //достигнутая точность
			double h, k;                      //шаги по x и y
			double h2, k2, a2;                //коэффициенты метода
			double Rn = 0.0;                  //невязка на последнем шаге
			double R0 = 0.0;                  //невязка на начальном шаге
			double z_max = 0.0;               //максимальная погрешность
			double x_max = 0.0, y_max = 0.0;  //макс погрешность в узле

			double v_old, v_new;
			double eps_curr = 0.0;

			double[][] v;  //массив численного решения
			double[][] u;  // массив точного решения
			double[][] f;  // массив правой части
			double[][] R;  // массив невязки
			double[][] Ar; // массив для ммн и мсг
			double[][] hn; // массив сопряженных направлений
			double[] T;    // массив параметров для метода чебышева

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

			v = new double[n + 1][];
			f = new double[n + 1][];
			u = new double[n + 1][];
			R = new double[n + 1][];
			Ar = new double[n + 1][];
			hn = new double[n + 1][];

			for (int i = 0; i <= n; i++)
			{
				v[i] = new double[m + 1];
				f[i] = new double[m + 1];
				u[i] = new double[m + 1];
				R[i] = new double[m + 1];
				Ar[i] = new double[m + 1];
				hn[i] = new double[m + 1];
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
				{
					double xi, yj;
					xi = a + i * h;
					yj = c + j * k;
					R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f_test(xi, yj);
				}

					R0 = 0.0;
			for (int i = 0; i < n + 1; i++)
				for (int j = 0; j < m + 1; j++)
					R0 += R[i][j] * R[i][j];
			R0 = Math.Sqrt(R0);

			double lambda1, lambdaN; // оценка собственных чисел
			double chisl = 0.0, znam = 0.0;

			// мпи
			if (radioButton14.Checked)
			{
				lambda1 = -4 * h2 * Math.Pow(Math.Sin(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Sin(Math.PI / (2.0 * m)), 2);
				lambdaN = -4 * h2 * Math.Pow(Math.Cos(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Cos(Math.PI / (2.0 * m)), 2);
				tau = 2 / (lambda1 + lambdaN);

				while (true)
				{
					for (int j = 1; j < m; j++)
						for (int i = 1; i < n; i++)
							R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
					eps_max = 0.0;
					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							v_old = v[i][j];
							v_new = v_old - tau * R[i][j];

							eps_curr = Math.Abs(v_old - v_new);
							if (eps_curr > eps_max) { eps_max = eps_curr; };
							v[i][j] = v_new;
						}
					}
					s++;
					if ((eps_max < eps) || (s > n_max))
						break;
				}

				label6.Text = "Метод:    метод простой итерации";
				label7.Text = "Параметр метода:    " + tau.ToString();
			}
			// ммн
			if (radioButton13.Checked)
			{
				while (true)
				{
					chisl = 0.0; znam = 0.0;
					for (int j = 1; j < m; j++)
						for (int i = 1; i < n; i++)
						{
							R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
							chisl += (a2 * R[i][j] + h2 * R[i + 1][j] + h2 * R[i - 1][j] + k2 * R[i][j + 1] + k2 * R[i][j - 1]) * R[i][j];
							znam += Math.Pow((a2 * R[i][j] + h2 * R[i + 1][j] + h2 * R[i - 1][j] + k2 * R[i][j + 1] + k2 * R[i][j - 1]), 2);
						}
					tau = chisl / znam;
					eps_max = 0.0;
					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							v_old = v[i][j];
							v_new = v_old - tau * R[i][j];

							eps_curr = Math.Abs(v_old - v_new);
							if (eps_curr > eps_max) { eps_max = eps_curr; };
							v[i][j] = v_new;
						}
					}
					s++;
					if ((eps_max < eps) || (s >= n_max))
						break;
				}
				label6.Text = "Метод:    метод минимальных невязок";
			}
			// мчеб(к)
			if (radioButton12.Checked)
			{
				K = Convert.ToInt32(textBox12.Text);
				lambda1 = -4 * h2 * Math.Pow(Math.Sin(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Sin(Math.PI / (2.0 * m)), 2);
				lambdaN = -4 * h2 * Math.Pow(Math.Cos(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Cos(Math.PI / (2.0 * m)), 2);

				T = new double[K];
				for (int i = 0; i < K; i++)
				{
					T[i] = 2 / (lambdaN + lambda1 + (lambdaN - lambda1) * Math.Cos(Math.PI * (2 * i + 1) / (2.0 * K)));
				}

				int index = 0;
				while (true)
				{
					if (index < K - 1)
					{
						for (int j = 1; j < m; j++)
							for (int i = 1; i < n; i++)
								R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

								eps_max = 0.0;
						for (int j = 1; j < m; j++)
						{
							for (int i = 1; i < n; i++)
							{
								v_old = v[i][j];
								v_new = v_old - T[index] * R[i][j];

								eps_curr = Math.Abs(v_old - v_new);
								if (eps_curr > eps_max) { eps_max = eps_curr; };
								v[i][j] = v_new;
							}
						}
						index++;
					}
					else
					{
						for (int j = 1; j < m; j++)
							for (int i = 1; i < n; i++)
								R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

						eps_max = 0.0;
						for (int j = 1; j < m; j++)
						{
							for (int i = 1; i < n; i++)
							{
								v_old = v[i][j];
								v_new = v_old - T[index] * R[i][j];

								eps_curr = Math.Abs(v_old - v_new);
								if (eps_curr > eps_max) { eps_max = eps_curr; };
								v[i][j] = v_new;
							}
						}
						index = 0;
					}
					s++;
					if ((eps_max < eps) || (s > n_max))
						break;
				}
				label6.Text = "Метод:    метод с чебышевским набором параметров";
			}
			// мсг
			if (radioButton11.Checked)
			{
				double alfa, betta;
				bool flag = false;
				//первая итерация
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
						hn[i][j] = -R[i][j];
					}
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						chisl += R[i][j] * hn[i][j];
						znam += (a2 * hn[i][j] + h2 * hn[i + 1][j] + h2 * hn[i - 1][j] + k2 * hn[i][j + 1] + k2 * hn[i][j - 1]) * hn[i][j];
					}
				alfa = -chisl / znam;
				eps_max = 0.0;
				for (int j = 1; j < m; j++)
				{
					for (int i = 1; i < n; i++)
					{
						v_old = v[i][j];
						v_new = v_old + alfa * hn[i][j];

						eps_curr = Math.Abs(v_old - v_new);
						if (eps_curr > eps_max) { eps_max = eps_curr; };
						v[i][j] = v_new;
					}
				}
				s++;
				if ((eps_max < eps) || (s >= n_max))
					flag = true;
				// вторая итерация и далее
				while (flag != true)
				{
					chisl = znam = 0.0;
					eps_max = 0.0;

					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
							chisl += (a2 * hn[i][j] + h2 * (hn[i + 1][j] + hn[i - 1][j]) + k2 * (hn[i][j + 1] + hn[i][j - 1])) * R[i][j];
							znam += (a2 * hn[i][j] + h2 * (hn[i + 1][j] + hn[i - 1][j]) + k2 * (hn[i][j + 1] + hn[i][j - 1])) * hn[i][j];
						}
					}

					betta = chisl / znam;

					chisl = znam = 0.0;

					for (int j = 0; j <= m; j++)
					{
						for (int i = 0; i <= n; i++)
						{
							hn[i][j] = -R[i][j] + betta * hn[i][j];
						}
					}

					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							chisl += R[i][j] * hn[i][j];
							znam += (a2 * hn[i][j] + h2 * hn[i + 1][j] + h2 * hn[i - 1][j] + k2 * hn[i][j + 1] + k2 * hn[i][j - 1]) * hn[i][j];
						}
					}

					alfa = -chisl / znam;

					chisl = znam = 0.0;

					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							v_old = v[i][j];

							v_new = v_old + alfa * hn[i][j];


							eps_curr = Math.Abs(v_old - v_new);
							if (eps_curr > eps_max) { eps_max = eps_curr; };
							v[i][j] = v_new;
						}
					}
					s++;
					if ((eps_max < eps) || (s >= n_max))
						flag = true;
				}
				label6.Text = "Метод:    метод сопряженных градиентов";
			}

			// невязка на последнем шаге
			for (int j = 1; j < m; j++)
				for (int i = 1; i < n; i++)
					R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

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
			label8.Text = "Достигнутая точность метода:" + "   " + eps_max.ToString();
			label9.Text = "Количество итераций:" + "   " + s.ToString();
			label5.Text = "Начальная невязка R0:" + "   " + (Math.Round(R0 * 1e8) / 1e8).ToString();
			label33.Text = "Невязка Rn:" + "   " + Rn.ToString();
			label10.Text = "Достигнутая погрешность: " + "   " + z_max.ToString();
			label12.Text = "Максимальная разность точного и численного решений в узле:" + "   (" + (Math.Round(x_max * 1e8) / 1e8).ToString() + ", " + (Math.Round(y_max * 1e8) / 1e8).ToString() + ")";

		}

		private void button2_Click(object sender, EventArgs e)
        {
			int n, m, n2, m2;       //размерность сетки
			int n_max;              //количество итераций
			double eps;             //заданная точность
			double a, b, c, d;      //границы участка
			int K;                  //количество параметров чебышева
			double tau;             //параметр мпи

			int s = 0, s2 = 0;                      //текущее кол-во итераций
			double eps_max = 0.0, eps_max2 = 0.0;       //достигнутая точность
			double h, k, _h, _k;                    //шаги по x и y
			double h2, k2, a2, _h2, _k2, _a2;       //коэффициенты метода
			double Rn = 0.0, Rn2 = 0.0;             //невязка
			double R0 = 0.0, R02 = 0.0;             //невязка на начальном шаге
			double z_max = 0.0;                     //максимальная погрешность
			double x_max = 0.0, y_max = 0.0;            //макс погрешность в узле

			double v_old, v_new;
			double eps_curr = 0.0, eps_curr2 = 0.0;

			double[][] v;   //массив численного решения
			double[][] v2;  // массив точного решения
			double[][] f;   // массив правой части
			double[][] f2;  // массив правой части 2
			double[][] R;   // массив невязки
			double[][] R2;  // массив невязки
			double[][] hn;  // массив для ммн и мсг
			double[][] hn2; // массив сопряженных направлений
			double[] T;     // массив параметров для метода чебышева

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

			v = new double[n + 1][];
			f = new double[n + 1][];
			f2 = new double[n2 + 1][];
			v2 = new double[n2 + 1][];
			R = new double[n + 1][];
			R2 = new double[n2 + 1][];
			hn = new double[n + 1][];
			hn2 = new double[n2 + 1][];

			for (int i = 0; i <= n; i++)
			{
				v[i] = new double[m + 1];
				f[i] = new double[m + 1];
				R[i] = new double[m + 1];
				hn[i] = new double[m + 1];
			}
			for (int i = 0; i <= n2; i++)
			{
				f2[i] = new double[m2 + 1];
				v2[i] = new double[m2 + 1];
				R2[i] = new double[m2 + 1];
				hn2[i] = new double[m2 + 1];
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

			double lambda1, lambdaN; // оценка собственных чисел
			double chisl = 0.0, znam = 0.0;

			// мпи
			if (radioButton7.Checked)
			{
				lambda1 = -4 * h2 * Math.Pow(Math.Sin(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Sin(Math.PI / (2.0 * m)), 2);
				lambdaN = -4 * h2 * Math.Pow(Math.Cos(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Cos(Math.PI / (2.0 * m)), 2);
				tau = 2 / (lambda1 + lambdaN);

				while (true)
				{
					for (int j = 1; j < m; j++)
						for (int i = 1; i < n; i++)
							R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
					eps_max = 0.0;
					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							v_old = v[i][j];
							v_new = v_old - tau * R[i][j];

							eps_curr = Math.Abs(v_old - v_new);
							if (eps_curr > eps_max) { eps_max = eps_curr; };
							v[i][j] = v_new;
						}
					}
					s++;
					if ((eps_max < eps) || (s > n_max))
						break;
				}

				label15.Text = "Метод:    метод простой итерации";
				label16.Text = "Параметр метода:    " + tau.ToString();
			}
			// ммн
			if (radioButton8.Checked)
			{
				while (true)
				{
					chisl = 0.0; znam = 0.0;
					for (int j = 1; j < m; j++)
						for (int i = 1; i < n; i++)
						{
							R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
							chisl += (a2 * R[i][j] + h2 * R[i + 1][j] + h2 * R[i - 1][j] + k2 * R[i][j + 1] + k2 * R[i][j - 1]) * R[i][j];
							znam += Math.Pow((a2 * R[i][j] + h2 * R[i + 1][j] + h2 * R[i - 1][j] + k2 * R[i][j + 1] + k2 * R[i][j - 1]), 2);
						}
					tau = chisl / znam;
					eps_max = 0.0;
					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							v_old = v[i][j];
							v_new = v_old - tau * R[i][j];

							eps_curr = Math.Abs(v_old - v_new);
							if (eps_curr > eps_max) { eps_max = eps_curr; };
							v[i][j] = v_new;
						}
					}
					s++;
					if ((eps_max < eps) || (s >= n_max))
						break;
				}
				label15.Text = "Метод:    метод минимальных невязок";
				label16.Text = " ";
			}
			// мчеб(к)
			if (radioButton9.Checked)
			{
				K = Convert.ToInt32(textBox11.Text);
				lambda1 = -4 * h2 * Math.Pow(Math.Sin(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Sin(Math.PI / (2.0 * m)), 2);
				lambdaN = -4 * h2 * Math.Pow(Math.Cos(Math.PI / (2.0 * n)), 2) - 4 * k2 * Math.Pow(Math.Cos(Math.PI / (2.0 * m)), 2);

				T = new double[K];
				for (int i = 0; i < K; i++)
				{
					T[i] = 2 / (lambdaN + lambda1 + (lambdaN - lambda1) * Math.Cos(Math.PI * (2 * i + 1) / (2.0 * K)));
				}

				int index = 0;
				while (true)
				{
					if (index < K - 1)
					{
						for (int j = 1; j < m; j++)
							for (int i = 1; i < n; i++)
								R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

						eps_max = 0.0;
						for (int j = 1; j < m; j++)
						{
							for (int i = 1; i < n; i++)
							{
								v_old = v[i][j];
								v_new = v_old - T[index] * R[i][j];

								eps_curr = Math.Abs(v_old - v_new);
								if (eps_curr > eps_max) { eps_max = eps_curr; };
								v[i][j] = v_new;
							}
						}
						index++;
					}
					else
					{
						for (int j = 1; j < m; j++)
							for (int i = 1; i < n; i++)
								R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

						eps_max = 0.0;
						for (int j = 1; j < m; j++)
						{
							for (int i = 1; i < n; i++)
							{
								v_old = v[i][j];
								v_new = v_old - T[index] * R[i][j];

								eps_curr = Math.Abs(v_old - v_new);
								if (eps_curr > eps_max) { eps_max = eps_curr; };
								v[i][j] = v_new;
							}
						}
						index = 0;
					}
					s++;
					if ((eps_max < eps) || (s > n_max))
						break;
				}
				label15.Text = "Метод:    метод с чебышевским набором параметров";
				label16.Text = " ";
			}
			// мсг
			if (radioButton10.Checked)
			{
				double alfa, betta;
				bool flag = false;
				//первая итерация
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
						hn[i][j] = -R[i][j];
					}
				for (int j = 1; j < m; j++)
					for (int i = 1; i < n; i++)
					{
						chisl += R[i][j] * hn[i][j];
						znam += (a2 * hn[i][j] + h2 * hn[i + 1][j] + h2 * hn[i - 1][j] + k2 * hn[i][j + 1] + k2 * hn[i][j - 1]) * hn[i][j];
					}
				alfa = -chisl / znam;
				eps_max = 0.0;
				for (int j = 1; j < m; j++)
				{
					for (int i = 1; i < n; i++)
					{
						v_old = v[i][j];
						v_new = v_old + alfa * hn[i][j];

						eps_curr = Math.Abs(v_old - v_new);
						if (eps_curr > eps_max) { eps_max = eps_curr; };
						v[i][j] = v_new;
					}
				}
				s++;
				if ((eps_max < eps) || (s >= n_max))
					flag = true;
				// вторая итерация и далее
				while (flag != true)
				{
					chisl = znam = 0.0;
					eps_max = 0.0;

					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];
							chisl += (a2 * hn[i][j] + h2 * (hn[i + 1][j] + hn[i - 1][j]) + k2 * (hn[i][j + 1] + hn[i][j - 1])) * R[i][j];
							znam += (a2 * hn[i][j] + h2 * (hn[i + 1][j] + hn[i - 1][j]) + k2 * (hn[i][j + 1] + hn[i][j - 1])) * hn[i][j];
						}
					}

					betta = chisl / znam;

					chisl = znam = 0.0;

					for (int j = 0; j <= m; j++)
					{
						for (int i = 0; i <= n; i++)
						{
							hn[i][j] = -R[i][j] + betta * hn[i][j];
						}
					}

					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							chisl += R[i][j] * hn[i][j];
							znam += (a2 * hn[i][j] + h2 * hn[i + 1][j] + h2 * hn[i - 1][j] + k2 * hn[i][j + 1] + k2 * hn[i][j - 1]) * hn[i][j];
						}
					}

					alfa = -chisl / znam;

					chisl = znam = 0.0;

					for (int j = 1; j < m; j++)
					{
						for (int i = 1; i < n; i++)
						{
							v_old = v[i][j];

							v_new = v_old + alfa * hn[i][j];
							eps_curr = Math.Abs(v_old - v_new);
							if (eps_curr > eps_max) { eps_max = eps_curr; };
							v[i][j] = v_new;
						}
					}
					s++;
					if ((eps_max < eps) || (s >= n_max))
						flag = true;
				}
				label15.Text = "Метод:    метод сопряженных градиентов";
				label16.Text = " ";
			}

			// мпи
			if (radioButton11.Checked)
			{
				lambda1 = -4 * _h2 * Math.Pow(Math.Sin(Math.PI / (2.0 * n2)), 2) - 4 * _k2 * Math.Pow(Math.Sin(Math.PI / (2.0 * m2)), 2);
				lambdaN = -4 * _h2 * Math.Pow(Math.Cos(Math.PI / (2.0 * n2)), 2) - 4 * _k2 * Math.Pow(Math.Cos(Math.PI / (2.0 * m2)), 2);
				tau = 2 / (lambda1 + lambdaN);

				while (true)
				{
					for (int j = 1; j < m2; j++)
						for (int i = 1; i < n2; i++)
							R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];
					eps_max2 = 0.0;
					for (int j = 1; j < m2; j++)
					{
						for (int i = 1; i < n2; i++)
						{
							v_old = v2[i][j];
							v_new = v_old - tau * R2[i][j];

							eps_curr2 = Math.Abs(v_old - v_new);
							if (eps_curr2 > eps_max2) { eps_max2 = eps_curr2; };
							v2[i][j] = v_new;
						}
					}
					s2++;
					if ((eps_max2 < eps) || (s2 > n_max))
						break;
				}

				label13.Text = "Метод:    метод простой итерации";
				label14.Text = "Параметр метода:    " + tau.ToString();
			}
			// ммн
			if (radioButton10.Checked)
			{
				while (true)
				{
					chisl = 0.0; znam = 0.0;
					for (int j = 1; j < m2; j++)
						for (int i = 1; i < n2; i++)
						{
							R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];
							chisl += (_a2 * R2[i][j] + _h2 * R2[i + 1][j] + _h2 * R2[i - 1][j] + _k2 * R2[i][j + 1] + _k2 * R2[i][j - 1]) * R2[i][j];
							znam += Math.Pow((_a2 * R2[i][j] + _h2 * R2[i + 1][j] + _h2 * R2[i - 1][j] + _k2 * R2[i][j + 1] + _k2 * R2[i][j - 1]), 2);
						}
					tau = chisl / znam;
					eps_max2 = 0.0;
					for (int j = 1; j < m2; j++)
					{
						for (int i = 1; i < n2; i++)
						{
							v_old = v2[i][j];
							v_new = v_old - tau * R2[i][j];

							eps_curr2 = Math.Abs(v_old - v_new);
							if (eps_curr2 > eps_max2) { eps_max2 = eps_curr2; };
							v2[i][j] = v_new;
						}
					}
					s2++;
					if ((eps_max2 < eps) || (s2 >= n_max))
						break;
				}
				label13.Text = "Метод:    метод минимальных невязок";
				label14.Text = "   ";
			}
			// мчеб(к)
			if (radioButton9.Checked)
			{
				K = Convert.ToInt32(textBox11.Text);
				lambda1 = -4 * _h2 * Math.Pow(Math.Sin(Math.PI / (2.0 * n2)), 2) - 4 * _k2 * Math.Pow(Math.Sin(Math.PI / (2.0 * m2)), 2);
				lambdaN = -4 * _h2 * Math.Pow(Math.Cos(Math.PI / (2.0 * n2)), 2) - 4 * _k2 * Math.Pow(Math.Cos(Math.PI / (2.0 * m2)), 2);

				T = new double[K];
				for (int i = 0; i < K; i++)
				{
					T[i] = 2 / (lambdaN + lambda1 + (lambdaN - lambda1) * Math.Cos(Math.PI * (2 * i + 1) / (2.0 * K)));
				}

				int index = 0;
				while (true)
				{
					if (index < K - 1)
					{
						for (int j = 1; j < m2; j++)
							for (int i = 1; i < n2; i++)
								R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];

						eps_max2 = 0.0;
						for (int j = 1; j < m2; j++)
						{
							for (int i = 1; i < n2; i++)
							{
								v_old = v2[i][j];
								v_new = v_old - T[index] * R2[i][j];

								eps_curr2 = Math.Abs(v_old - v_new);
								if (eps_curr2 > eps_max2) { eps_max2 = eps_curr2; };
								v2[i][j] = v_new;
							}
						}
						index++;
					}
					else
					{
						for (int j = 1; j < m2; j++)
							for (int i = 1; i < n2; i++)
								R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];

						eps_max2 = 0.0;
						for (int j = 1; j < m2; j++)
						{
							for (int i = 1; i < n2; i++)
							{
								v_old = v2[i][j];
								v_new = v_old - T[index] * R2[i][j];

								eps_curr2 = Math.Abs(v_old - v_new);
								if (eps_curr2 > eps_max2) { eps_max2 = eps_curr2; };
								v2[i][j] = v_new;
							}
						}
						index = 0;
					}
					s2++;
					if ((eps_max2 < eps) || (s2 > n_max))
						break;
				}
				label13.Text = "Метод:    метод с чебышевским набором параметров";
				label14.Text = "   ";
			}
			// мсг
			if (radioButton8.Checked)
			{
				double alfa, betta;
				bool flag = false;
				//первая итерация
				for (int j = 1; j < m2; j++)
					for (int i = 1; i < n2; i++)
					{
						R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];
						hn2[i][j] = -R2[i][j];
					}
				for (int j = 1; j < m2; j++)
					for (int i = 1; i < n2; i++)
					{
						chisl += R2[i][j] * hn2[i][j];
						znam += (_a2 * hn2[i][j] + _h2 * hn2[i + 1][j] + _h2 * hn2[i - 1][j] + _k2 * hn2[i][j + 1] + _k2 * hn2[i][j - 1]) * hn2[i][j];
					}
				alfa = -chisl / znam;
				eps_max2 = 0.0;
				for (int j = 1; j < m2; j++)
				{
					for (int i = 1; i < n2; i++)
					{
						v_old = v2[i][j];
						v_new = v_old + alfa * hn2[i][j];

						eps_curr2 = Math.Abs(v_old - v_new);
						if (eps_curr2 > eps_max2) { eps_max2 = eps_curr2; };
						v2[i][j] = v_new;
					}
				}
				s2++;
				if ((eps_max2 < eps) || (s2 >= n_max))
					flag = true;
				// вторая итерация и далее
				while (flag != true)
				{
					chisl = znam = 0.0;
					eps_max2 = 0.0;

					for (int j = 1; j < m2; j++)
					{
						for (int i = 1; i < n2; i++)
						{
							R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];
							chisl += (_a2 * hn2[i][j] + _h2 * (hn2[i + 1][j] + hn2[i - 1][j]) + _k2 * (hn2[i][j + 1] + hn2[i][j - 1])) * R2[i][j];
							znam += (_a2 * hn2[i][j] + _h2 * (hn2[i + 1][j] + hn2[i - 1][j]) + _k2 * (hn2[i][j + 1] + hn2[i][j - 1])) * hn2[i][j];
						}
					}

					betta = chisl / znam;

					chisl = znam = 0.0;

					for (int j = 0; j <= m2; j++)
					{
						for (int i = 0; i <= n2; i++)
						{
							hn2[i][j] = -R2[i][j] + betta * hn2[i][j];
						}
					}

					for (int j = 1; j < m2; j++)
					{
						for (int i = 1; i < n2; i++)
						{
							chisl += R2[i][j] * hn2[i][j];
							znam += (_a2 * hn2[i][j] + _h2 * hn2[i + 1][j] + _h2 * hn2[i - 1][j] + _k2 * hn2[i][j + 1] + _k2 * hn2[i][j - 1]) * hn2[i][j];
						}
					}

					alfa = -chisl / znam;

					chisl = znam = 0.0;

					for (int j = 1; j < m2; j++)
					{
						for (int i = 1; i < n2; i++)
						{
							v_old = v2[i][j];
							v_new = v_old + alfa * hn2[i][j];
							eps_curr2 = Math.Abs(v_old - v_new);
							if (eps_curr2 > eps_max2) { eps_max2 = eps_curr2; };
							v2[i][j] = v_new;
						}
					}
					s2++;
					if ((eps_max2 < eps) || (s2 >= n_max))
						flag = true;
				}
				label13.Text = "Метод:    метод сопряженных градиентов";
				label14.Text = "   ";
			}

			// невязка на последнем шаге
			for (int j = 1; j < m; j++)
				for (int i = 1; i < n; i++)
					R[i][j] = a2 * v[i][j] + h2 * v[i + 1][j] + h2 * v[i - 1][j] + k2 * v[i][j + 1] + k2 * v[i][j - 1] - f[i][j];

			Rn = 0.0;
			for (int i = 0; i < n + 1; i++)
				for (int j = 0; j < m + 1; j++)
					Rn += R[i][j] * R[i][j];
			Rn = Math.Sqrt(Rn);

			for (int j = 1; j < m2; j++)
				for (int i = 1; i < n2; i++)
					R2[i][j] = _a2 * v2[i][j] + _h2 * v2[i + 1][j] + _h2 * v2[i - 1][j] + _k2 * v2[i][j + 1] + _k2 * v2[i][j - 1] - f2[i][j];

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
			FileStream fs = new FileStream("numeric1.csv", FileMode.Create);
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
			label19.Text = "Достигнутая точность метода:" + "   " + eps_max.ToString();
			label18.Text = "Количество итераций:" + "   " + s.ToString();
			label20.Text = "Начальная невязка R0:" + "   " + (Math.Round(R0 * 1e8) / 1e8).ToString();
			label17.Text = "Невязка Rn:" + "   " + Rn.ToString();

			label28.Text = "Достигнутая точность метода:" + "   " + eps_max2.ToString();
			label30.Text = "Количество итераций:" + "   " + s2.ToString();
			label32.Text = "Начальная невязка R0:" + "   " + (Math.Round(R02 * 1e8) / 1e8).ToString();
			label27.Text = "Невязка Rn:" + "   " + Rn2.ToString();

			label26.Text = "Достигнутая точность решения: " + "   " + z_max.ToString();
			label25.Text = "Максимальное отклонение численных решений в узле::" + "   (" + (Math.Round(x_max * 1e8) / 1e8).ToString() + ", " + (Math.Round(y_max * 1e8) / 1e8).ToString() + ")";

		}
	}
}
