using BinaryCalculator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba1
{
    public partial class Form1 : Form
    {

        private Calculator calculator = new Calculator();

        public Form1()
        {
            InitializeComponent();
        }

        private void ShowResult(int result)
        {
            lblDec.Text = Convert.ToString(result);
            lblBin.Text = Convert.ToString(result, 2);
            lblOct.Text = Convert.ToString(result, 8);
            lblHex.Text = Convert.ToString(result, 16);
        }

        private void btnAnd_Click(object sender, EventArgs e)
        {
            try
            {
                int a = int.Parse(textA.Text);
                int b = int.Parse(textB.Text);
                calculator.OnBinaryOperation += calculator.And;

                int result = calculator.ExecuteBinary(a, b);

                ShowResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
            finally
            {
                calculator.OnBinaryOperation -= calculator.And;
            }
        }

        private void btnOr_Click(object sender, EventArgs e)
        {
            try
            {
                int a = int.Parse(textA.Text);
                int b = int.Parse(textB.Text);
                calculator.OnBinaryOperation += calculator.Or;

                int result = calculator.ExecuteBinary(a, b);

                ShowResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
            finally
            {
                calculator.OnBinaryOperation -= calculator.Or;
            }
        }
        private void btnXor_Click(object sender, EventArgs e)
        {
            try
            {
                int a = int.Parse(textA.Text);
                int b = int.Parse(textB.Text);
                calculator.OnBinaryOperation += calculator.Xor;

                int result = calculator.ExecuteBinary(a, b);

                ShowResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
            finally
            {
                calculator.OnBinaryOperation -= calculator.Xor;
            }
        }
        private void btnNotA_Click(object sender, EventArgs e)
        {
            try
            {
                int a = int.Parse(textA.Text);
                calculator.OnUnaryOperation += calculator.Not;

                int result = calculator.ExecuteUnary(a);

                ShowResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
            finally
            {
                calculator.OnUnaryOperation -= calculator.Not;
            }
        }

        private void btnNotB_Click(object sender, EventArgs e)
        {
            try
            {
                int b = int.Parse(textB.Text);
                calculator.OnUnaryOperation += calculator.Not;

                int result = calculator.ExecuteUnary(b);

                ShowResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
            finally
            {
                calculator.OnUnaryOperation -= calculator.Not;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textA.Clear();
            textB.Clear();
            lblBin.Text = "";
            lblDec.Text = "";
            lblHex.Text = "";
            lblOct.Text = "";
        }

        private void label18_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Привет");
        }
    }
}
