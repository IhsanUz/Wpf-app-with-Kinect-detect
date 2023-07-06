using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Windows.Threading;

namespace Wpf_with_Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _sensor = null;
        private ColorFrameReader _colorReader = null;
        private BodyFrameReader _bodyReader = null;
        private IList<Body> _bodies = null;

        private int _width = 0;
        private int _height = 0;
        private byte[] _pixels = null;
        private WriteableBitmap _bitmap = null;
        //variables
        private bool grab_one = false;
        private bool hold = false;
        bool grab_potato = false;
        bool grab_onion = false;
        public int count; //play sound counter
        public int score_point; //score counter
       
        private void onion_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
          /* if ((Canvas.GetLeft(hand) >= Canvas.GetLeft(take_onion) - 39 && Canvas.GetLeft(hand) <= Canvas.GetLeft(take_onion) - 28) && (Canvas.GetTop(hand) >= Canvas.GetTop(take_onion) + 16 && Canvas.GetTop(hand) <= Canvas.GetTop(take_onion) + 46))
            {
                hand.Source = new BitmapImage(new Uri("/Assets/hold_item.png", UriKind.Relative));
                hold = true;
            }*/
        }
        private void potato_hold()
        {
            if (!hold)
            {
                hold = true;
                score_point += 3;
                ScoreScreen(score_point);
                grab_potato = true;
            }         
        }
        private void onion_hold()
        {
            if (!hold)
            {
                hold = true;
                score_point += 3;
                ScoreScreen(score_point);
                grab_onion = true;
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _width = _sensor.ColorFrameSource.FrameDescription.Width;
                _height = _sensor.ColorFrameSource.FrameDescription.Height;

                _colorReader = _sensor.ColorFrameSource.OpenReader();
                _colorReader.FrameArrived += ColorReader_FrameArrived;

                _bodyReader = _sensor.BodyFrameSource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;

                _pixels = new byte[_width * _height * 4];
                _bitmap = new WriteableBitmap(_width, _height, 96.0, 96.0, PixelFormats.Bgra32, null);

                _bodies = new Body[_sensor.BodyFrameSource.BodyCount];

              //  camera.Source = _bitmap;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_colorReader != null)
            {
                _colorReader.Dispose();
            }

            if (_bodyReader != null)
            {
                _bodyReader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        private void ColorReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.CopyConvertedFrameDataToArray(_pixels, ColorImageFormat.Bgra);

                    _bitmap.Lock();
                    Marshal.Copy(_pixels, 0, _bitmap.BackBuffer, _pixels.Length);
                    _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                    _bitmap.Unlock();
                }
            }
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(_bodies);

                    Body body = _bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (body != null)
                    {
                        Joint handRight = body.Joints[JointType.HandRight];
                    

                        if (handRight.TrackingState != TrackingState.NotTracked)
                        {
                            CameraSpacePoint handRightPosition = handRight.Position;
                            ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                            float x = handRightPoint.X;
                            float y = handRightPoint.Y;

                            if (!float.IsInfinity(x) && !float.IsInfinity(y))
                            {                           
                                // trail.Points.Add(new Point { X = x, Y = y });
                                //Adım 0
                                Canvas.SetLeft(hand, x- hand.Width / 2.0);
                                Canvas.SetTop(hand, y - hand.Height);
                                //Adım 1
                                if (grab_one)
                                {
                                    //Patates'i el hizesına göre hareket ettirmek
                                    if (grab_potato)
                                    {
                                        Canvas.SetLeft(potato_figüre, Canvas.GetLeft(hand) - 27);
                                        Canvas.SetTop(potato_figüre, Canvas.GetTop(hand) - 26);
                                        check_rectangle_area();
                                    }
                                    else
                                    {
                                        Canvas.SetLeft(Onion_figüre, Canvas.GetLeft(hand) + 21);
                                        Canvas.SetTop(Onion_figüre, Canvas.GetTop(hand) - 9);
                                        check_rectangle_area();
                                    }
                                }
                                //Adım 2
                           else if (task_potato.IsChecked == false && !grab_one && !hold && (Canvas.GetLeft(hand) >= Canvas.GetLeft(coordinate) && Canvas.GetLeft(hand) <= Canvas.GetLeft(fridge_potato))
                                   && (Canvas.GetTop(hand) >= Canvas.GetTop(coordinate) && Canvas.GetTop(hand) <= Canvas.GetTop(coordinate) + 29))
                                {
                                    hand.Source = new BitmapImage(new Uri("/Assets/hold_item.png", UriKind.Relative));
                                    Canvas.SetLeft(potato_figüre, Canvas.GetLeft(hand) - 27);
                                    Canvas.SetTop(potato_figüre, Canvas.GetTop(hand) - 26);
                                    grab_one = true;
                                    potato_hold();
                                }
                                //Adım 3
                           else if (task_onion.IsChecked == false && !grab_one && !hold && (Canvas.GetLeft(hand) >= Canvas.GetLeft(take_onion) - 39 && Canvas.GetLeft(hand) <= Canvas.GetLeft(take_onion) - 28) 
                                    && (Canvas.GetTop(hand) >= Canvas.GetTop(take_onion) + 16 && Canvas.GetTop(hand) <= Canvas.GetTop(take_onion) + 46))
                                {
                                    hand.Source = new BitmapImage(new Uri("/Assets/hold_item.png", UriKind.Relative));
                                    Canvas.SetLeft(Onion_figüre, Canvas.GetLeft(hand) + 21);
                                    Canvas.SetTop(Onion_figüre, Canvas.GetTop(hand) - 9);
                                    grab_one = true;
                                    onion_hold();
                                }                            
                            }                          
                        }                     
                    }
                }
            }
        }    
        private void check_rectangle_area()
        {
            //Adım 4
            if (grab_potato && (Canvas.GetLeft(potato_area) + 13 <= Canvas.GetLeft(potato_figüre) && Canvas.GetLeft(potato_area) + 35 >= Canvas.GetLeft(potato_figüre)) &&
                (Canvas.GetTop(potato_area) - 11 <= Canvas.GetTop(potato_figüre) && Canvas.GetTop(potato_area) + 46 >= Canvas.GetTop(potato_figüre)))
            {
                text_potato.Text = "Excellent";
                score_point += 7;
                count++;
                sound_source(count);
                ScoreScreen(score_point);
                task_potato.IsChecked = true;
                alltask_Checked();
                grab_one = false;
                hold = false;
                grab_potato = false;
                hand.Source = new BitmapImage(new Uri("/Assets/hand.png", UriKind.Relative));
            }
            //Adım 5
       else if (grab_onion && (Canvas.GetLeft(onion_area) + 9 <= Canvas.GetLeft(Onion_figüre) && Canvas.GetLeft(onion_area) + 36 >= Canvas.GetLeft(Onion_figüre)) &&
                (Canvas.GetTop(onion_area) +5 <= Canvas.GetTop(Onion_figüre) && Canvas.GetTop(onion_area) + 31 >= Canvas.GetTop(Onion_figüre)))
            {
                text_onion.Text = "Excellent";
                score_point += 7;
                count++;
                sound_source(count);
                ScoreScreen(score_point);
                task_onion.IsChecked = true;
                alltask_Checked();
                grab_one = false;
                hold = false;
                grab_onion = false;
                hand.Source = new BitmapImage(new Uri("/Assets/hand.png", UriKind.Relative));
            }
        }
        private void sound_source(int count)
        {
            var play_sound = new MediaPlayer();
            Uri new_effect;
            if (count == 1)
            {
                new_effect = new Uri(@"C:\Users\LENOVO\source\repos\Wpf_with_Kinect\Wpf_with_Kinect\ding.mp3");
                play_sound.Open(new_effect);
                play_sound.Play();

            }
            else if (count == 2)
            {
                new_effect = new Uri(@"C:\Users\LENOVO\source\repos\Wpf_with_Kinect\Wpf_with_Kinect\excellent_work.mp3");
                play_sound.Open(new_effect);
                play_sound.Play();
            }
            else if (count == 3)
            {
                new_effect = new Uri(@"C:\Users\LENOVO\source\repos\Wpf_with_Kinect\Wpf_with_Kinect\success.mp3");
                play_sound.Open(new_effect);
                play_sound.Play();
            }
            else
            {
                new_effect = new Uri(@"C:\Users\LENOVO\source\repos\Wpf_with_Kinect\Wpf_with_Kinect\fruit_drop.mp3");
                play_sound.Open(new_effect);
                play_sound.Play();
            }
        }
        private void alltask_Checked()
        {
            if (task_onion.IsChecked == true && task_potato.IsChecked == true)
            {
                wait();
                alltask.IsChecked = true;
                count += 1;
                sound_source(count);
                score_point += 5;
                ScoreScreen(score_point);
            }
        }
        private async void wait()
        {                  
                await System.Threading.Tasks.Task.Delay(1000);          
        }
        private void ScoreScreen(int count)
        {
            score_box.Text = "Score: " + count;
        }      
    }
}
