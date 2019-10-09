using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;



//References for FFT Analysis
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Numerics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using MaterialSkin;
using MaterialSkin.Controls;



namespace FFTAnalysis
{
    public partial class Form1 : MaterialForm
    {
        //global variable declaration
        static int numSamples = 1000;
        static int sampleRate = 2000;
        static int magSecond;
        static int magThird;
        static double PHSecond;
        static double PHthird;
        
        Complex[] samples = new Complex[numSamples]; // creating a complex array

        public Form1() // Consider it as the main method
        {
            InitializeComponent();
        }

        public void PlotWaveform(int secondHarm, int thirdHarm, double secondPH, double thridPH)
        {
            chart1.Series["Waveform"].Points.Clear();
            chart1.Series["Second Harmonic"].Points.Clear();
            chart1.Series["Third Harmonic"].Points.Clear();

            //Generate fundamental, 2nd & 3rd Harmonic Waveform using MathNET library
            double[] fundamental = Generate.Sinusoidal(numSamples, sampleRate, 60, 10.0);
            double[] second = Generate.Sinusoidal(numSamples, sampleRate, 120, secondHarm, 0.0, secondPH);
            double[] third = Generate.Sinusoidal(numSamples, sampleRate, 180, thirdHarm, 0.0, thridPH);

            //Add waveforms together to get composite waveforms
            for (int i = 0; i < numSamples; i++)
            {
                samples[i] = new Complex(fundamental[i] + second[i] + third[i], 0);
            }

            //Plot Composite Waveforms
            //Each sample represents a time of 1/sampling rate
            for (int i = 0; i < samples.Length / 5; i++)
            {
                double time = ((i + 1.0)) / numSamples / 2;

                chart1.Series["Waveform"].LegendText = "Waveform";
                chart1.Series["Waveform"].ChartType = SeriesChartType.Line;

                chart1.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Arial", 12.0f);
                chart1.ChartAreas["ChartArea1"].AxisX.Title = "Seconds";

                if (checkBox1.Checked)
                {
                    chart1.Series["Second Harmonic"].Points.AddXY(time, second[i]);
                }

                if (checkBox2.Checked)
                {
                    chart1.Series["Third Harmonic"].Points.AddXY(time, third[i]);
                }

                chart1.Series["Waveform"].Points.AddXY(time, samples[i].Real);


            }


        }

        public void PlotFFT()
        {
            chart2.Series["Frequency"].Points.Clear();
            Fourier.Forward(samples, FourierOptions.NoScaling); 
            // "Forward" Fourier converts time => Frequency

            //Plot the frequency spectrum. ("bidirectional bandwiddth"
            //You only need the bottom of 1/2 of the samples
            for (int i = 1; i < samples.Length / 2; i++)
            {
                chart2.ChartAreas["ChartArea1"].AxisX.Title = "HZ";
                chart2.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Arial", 12.0f);
                chart2.ChartAreas["ChartArea1"].AxisX.MinorTickMark.Enabled = true;

                //Get the maginitude of each FFT sample:
                //= abs[sqrt(r^2 + i^2)]
                double mag = (2.0 / numSamples) * (Math.Abs(Math.Sqrt(Math.Pow(samples[i].Real, 2) + Math.Pow(samples[i].Imaginary, 2))));

                //Deteremine how many HZ represetned by each sample
                double hzPerSample = sampleRate / numSamples;

                chart2.Series["Frequency"].Points.AddXY(hzPerSample * i, mag);

            }



        }


        private void Button2_Click(object sender, EventArgs e)
        {
            //EXIT Button
            Application.Exit();
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            //120HZ Magnitude
            trackBar1.Enabled = true;
            magSecond = trackBar1.Value;
            label1.Text = magSecond.ToString("F0");
            PlotWaveform(magSecond, magThird, PHSecond, PHthird);
            PlotFFT();
        }


        // Event Handlers

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            //180HZ Magnitude
            trackBar2.Enabled = true;
            magThird = trackBar2.Value;
            label2.Text = magThird.ToString("F0");
            PlotWaveform(magSecond, magThird, PHSecond, PHthird);
            PlotFFT();
        }

        private void TrackBar3_Scroll(object sender, EventArgs e)
        {
            //120 HZ Phase
            //Value goes from 0 to 10, corresponding to 0 to 360 degrees, or 2*PI radians
            //First Convert value to radians, where 10 = 360 degree or 2 PI radians
            //trackBar3.Enabled = true;
            PHSecond = 2.0*Math.PI*((trackBar3.Value) / (double)trackBar3.Maximum);
            double angle = PHSecond * 360.0/(2*Math.PI);
            label3.Text = angle.ToString("F1");
            PlotWaveform(magSecond, magThird, PHSecond, PHthird);
            PlotFFT();
        }

        private void TrackBar4_Scroll(object sender, EventArgs e)
        {
            //180 HZ Phase
            //trackBar4.Enabled = true;
            PHthird = 2.0*Math.PI*((trackBar4.Value) / (double)trackBar4.Maximum);
            double angle = PHthird * 360.0/(2*Math.PI);
            label4.Text = angle.ToString("F1");
            PlotWaveform(magSecond, magThird, PHSecond, PHthird);
            PlotFFT();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //Load Button
            PlotWaveform(0, 0, 0, 0);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //FFT Button
            PlotFFT();
        }

    }
}
