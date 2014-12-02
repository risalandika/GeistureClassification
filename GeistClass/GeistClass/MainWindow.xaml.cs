using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Leap;
using Vector = Leap.Vector;
using System.IO;

namespace GeistClass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ILeapEventDelegate
    {
        private Controller controller = new Controller();
        private LeapEventListener listener;
        delegate void LeapEventDelegate(string EventName);
        Vector palmPositionOld = new Vector(0,0,0);
        List<Vector> vectorPosition = new List<Vector>();
        bool markStart=false;
        bool markEnd=false;
        float idleTime;
        long timeOld=0;
        bool loading = false;
        float delayLoading = 0;
        Thread mainThread = null;
        bool appRun = true;
        int count=0;
        StreamWriter file;
        string className = "";
        bool learningMode = false; // false = reading, true = learning
        bool textboxFocus = false;

        public MainWindow()
        {
            InitializeComponent();
            this.controller = new Controller();
            this.listener = new LeapEventListener(this);
            controller.AddListener(listener);
            appRun = true;
        }

        public void LeapEventNotification(string EventName)
        {
            if (this.CheckAccess())
            {
                switch (EventName)
                {
                    case "onInit":
                        Debug.WriteLine("Init");
                        mainThread = new Thread(new ThreadStart(this.MainLoop));
                        mainThread.Start();
                        break;
                    case "onConnect":
                        this.connectHandler();
                        break;
                }
            }
            else
            {
                Dispatcher.Invoke(new LeapEventDelegate(LeapEventNotification), new object[] { EventName });
            }
        }

        private void MainLoop()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (appRun)
            {
                sw.Stop();
                newFrameHandler(this.controller.Frame(), sw.ElapsedMilliseconds/1000.000f);
                sw.Reset();
                sw.Start();
                Thread.Sleep(1);
            }
        }

        void connectHandler()
        {
            //this.controller.SetPolicy(Controller.PolicyFlag.POLICY_IMAGES);
            //this.controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
            //this.controller.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
        }

        //Handle setiap frame
        void newFrameHandler(Leap.Frame frame, float deltaTime)
        {
            //Console.WriteLine("ID : " + frame.Id.ToString());
            //Console.WriteLine("Finger Count : " + frame.Fingers.Count.ToString());
            float distanceVector = frame.Hands.Rightmost.PalmPosition.DistanceTo(palmPositionOld);
            //Console.WriteLine("Distance : " + distanceVector);
            if (frame.Hands.Count != 0)
            {
                palmPositionOld = frame.Hands.Rightmost.PalmPosition;
                changeColorButton(3);
                ButtonPositionAsPalm(frame.Hands.Rightmost.PalmPosition);
            }
            else
            {
                distanceVector = 0;
                changeColorButton(4);
            }

            if (textboxFocus)
            {
                idleTime = 0;
                if (markStart)
                {
                    loading = true;
                }
                else
                {
                    changeColorButton(0);
                    return;
                }
            }

            if (!loading)
            {
                delayLoading = 0;
                if (distanceVector < 2.0f)
                {
                    idleTime += deltaTime;
                    if (idleTime >= 0.3f)
                    {
                        if (frame.Hands.Count == 0)
                        {
                            if (markStart == true)
                            {
                                loading = true;
                            }
                        }
                        else if (vectorPosition.Count == 0)
                        {
                            markStart = true;
                            vectorPosition.Add(palmPositionOld);
                        }
                        else if (vectorPosition.Count == 1)
                        {
                            vectorPosition[0] = palmPositionOld;
                        }
                        else
                        {
                            loading = true;
                        }
                    }
                }
                else
                {
                    idleTime = 0;
                    if (markStart == true)
                    {
                        vectorPosition.Add(palmPositionOld);
                    }
                }
                if (markStart == false)
                {
                    changeColorButton(0);
                }
                else changeColorButton(1);
            }
            else
            {
                changeColorButton(2);
                markStart = false;
                loading = false;

                //Data Training
                if (learningMode)
                {
                    if (vectorPosition.Count > 26 && !className.Equals(""))
                    {
                        count++;
                        file = new StreamWriter("DataRaw\\" + className + "." + count + ".txt");
                        for(int i = 1 ; i < vectorPosition.Count ; i++)
                        {
                            Vector dir = vectorPosition[i] - vectorPosition[i - 1];
                            float degree = (float)(Math.Atan2(dir.z, dir.x) * 180) / (float)Math.PI;
                            if (degree < 0)
                                degree = 360 + degree;
                            file.WriteLine(degree);
                        }

                        file.Close();
                        file = new StreamWriter("DataSet\\" + className + "." + count + ".txt");
                        float skipPerPoint = vectorPosition.Count / (25.0f);
                        //Console.WriteLine(vectorPosition.Count + ": " + skipPerPoint);

                        int index = 0;
                        for (float i = 1; i < vectorPosition.Count; i += skipPerPoint)
                        {
                            index = (int)i;
                            Vector dir = vectorPosition[index] - vectorPosition[index - 1];
                            float degree = (float)(Math.Atan2(dir.z, dir.x) * 180) / (float)Math.PI;
                            if (degree < 0)
                                degree = 360 + degree;
                            file.WriteLine(degree);
                        }
                        if (index != vectorPosition.Count - 1)
                        {
                            index = vectorPosition.Count - 1;
                            Vector dir = vectorPosition[index] - vectorPosition[index - 1];
                            float degree = (float)(Math.Atan2(dir.z, dir.x) * 180) / (float)Math.PI;
                            if (degree < 0)
                                degree = 360 + degree;
                            file.WriteLine(degree);
                        }
                        file.Close();
                    }
                }
                else
                {
                    // Reading mode
                }
                vectorPosition.Clear();
                idleTime = 0;

                delayLoading += deltaTime;
                // if (delayLoading > 1.0f)
            }
        }

        public delegate void ChangeColorButtonThread(int state);

        void changeColorButtonThreadSafe(int state)
        {
            CounterSet.Content = "Input Number: " + count;
            switch (state)
            {
                case 0:
                    GuideButton.Source = new BitmapImage(new Uri(@"/Images/grey-button.png", UriKind.Relative));
                    break;
                case 1:
                    GuideButton.Source = new BitmapImage(new Uri(@"/Images/green-button.png", UriKind.Relative));
                    break;
                case 2:
                    GuideButton.Source = new BitmapImage(new Uri(@"/Images/ajax-loader.gif", UriKind.Relative));
                    break;
                case 3:
                    GuideButton.Visibility = Visibility.Visible;
                    break;
                case 4:
                    GuideButton.Visibility = Visibility.Hidden;
                    break;
            }
        }

        void changeColorButton(int state)
        {
            GuideButton.Dispatcher.Invoke(new ChangeColorButtonThread(changeColorButtonThreadSafe), new object[] { state } );
        }

        public delegate void ButtonPositionAsPalmThread(Vector v);

        void ButtonPositionAsPalm(Vector v)
        {
            GuideButton.Dispatcher.Invoke(new ButtonPositionAsPalmThread(ButtonPositionAsPalmSafe), new object[] { v });
        }

        void ButtonPositionAsPalmSafe(Vector v)
        {
            double velocity = 2;
            GuideButton.Margin = new Thickness(v.x * velocity, v.z * velocity, 0, 0);
        }

        private void TextBox_Classification_TextChanged(object sender, TextChangedEventArgs e)
        {
            className = TextBox_Classification.Text;
            //Console.WriteLine(className);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            learningMode = !learningMode;

            if (learningMode)
            {
                Mode.Content = "Learning";
                TextBox_Classification.Visibility = Visibility.Visible;
            }
            else
            {
                Mode.Content = "Reading";
                TextBox_Classification.Visibility = Visibility.Hidden;
            }
        }

        private void TextBox_Classification_GotFocus(object sender, RoutedEventArgs e)
        {
            textboxFocus = true;
        }

        private void TextBox_Classification_LostFocus(object sender, RoutedEventArgs e)
        {
            textboxFocus = false;
        }
    }
}
