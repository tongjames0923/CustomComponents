using CustomComponents.InPutBoxfoundation;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CustomComponents
{
    namespace MessageTool
    {
        /// <summary>
        /// 弹出窗口的按钮选择
        /// </summary>
        public enum MyMessageBoxButton
        {/// <summary>
         /// 显示确认与取消按钮
         /// </summary>
            ConfirmNO,
            /// <summary>
            /// 显示确认按钮
            /// </summary>
            Confirm
        }
        /// <summary>
        /// 弹窗返回的用户选择
        /// </summary>
        public enum MyMessageBoxResult
        {
            /// <summary>
            /// 用户点击了确认
            /// </summary>
            Comfirm,
            /// <summary>
            /// 用户点击了取消
            /// </summary>
            No,
            /// <summary>
            /// 用户点击了自定义按钮
            /// </summary>
            Buttons,
            /// <summary>
            /// 用户未点击按钮
            /// </summary>
            None
        }
        /// <summary>
        /// 暂时的提示框
        /// </summary>
        public class HelpTextLabel : Label
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="father">该控件的父元素</param>
            public HelpTextLabel(Grid father)
            {
                Content = "";
                backcolor = new SolidColorBrush(Color.FromArgb(30, 220, 237, 105));
                HorizontalAlignment = HorizontalAlignment.Stretch;
                VerticalAlignment = VerticalAlignment.Center;
                BorderThickness = new Thickness { Top = 0, Left = 0, Right = 0, Bottom = 0 };
                MinHeight = 20;
                father.Children.Add(this);
            }
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="father">该控件的父元素</param>
            public HelpTextLabel(Canvas father)
            {
                Content = "";
                backcolor = new SolidColorBrush(Color.FromArgb(30, 220, 237, 105));
                BorderThickness = new Thickness { Top = 0, Left = 0, Right = 0, Bottom = 0 };
                HorizontalAlignment = HorizontalAlignment.Center;
                VerticalAlignment = VerticalAlignment.Center;
                MinHeight = 20;
                father.Children.Add(this);
            }
            private Brush backcolor;
            /// <summary>
            /// 显示时的背景颜色
            /// </summary>
            public new Brush Background
            {
                get => backcolor;
                set => backcolor = value;
            }
            /// <summary>
            /// 当前的背景颜色
            /// </summary>
            public Brush NowBackground
            {
                get => base.Background;
                set => base.Background = value;
            }
            /// <summary>
            /// 设置内容
            /// </summary>
            public new object Content
            {
                get => base.Content;
                set
                {
                    base.Content = value;
                    ChangeSize(value.ToString());
                }
            }
            private void ChangeSize(string c)
            {
                int linenum = 1;
                double max = c.Length;
                int charnum = 0;
                foreach (char temp in c.ToCharArray())
                {
                    charnum++;
                    if (temp == '\n' || temp == '\r')
                    {
                        max = charnum;
                        charnum = 0;
                        linenum++;
                    }
                }
                Width = FontSize * max;
                Height = linenum * FontSize * 2;
            }
            private double ShowTime = 2;
            /// <summary>
            /// 提示框出现的持续时间（秒）
            /// </summary>
            public double Continued
            {
                get => ShowTime;
                set => ShowTime = value;
            }
            /// <summary>
            /// 初始化
            /// </summary>
            public HelpTextLabel()
            {
                Content = "";
                backcolor = new SolidColorBrush(Color.FromArgb(30, 220, 237, 105));
                BorderThickness = new Thickness { Top = 0, Left = 0, Right = 0, Bottom = 0 };
                HorizontalAlignment = HorizontalAlignment.Stretch;
                VerticalAlignment = VerticalAlignment.Center;
                MinHeight = 25;
            }
            /// <summary>
            /// 当出现时的操作
            /// </summary>
            public event EventHandler OnShow;
            /// <summary>
            /// 当消失时的操作
            /// </summary>
            public event EventHandler OnDisappear;
            private void ResetText(object sender, EventArgs e)
            {
                if (OnDisappear != null)
                {
                    OnDisappear(this, EventArgs.Empty);
                }

                Content = "";
                DispatcherTimer dispatcherTimer = sender as DispatcherTimer;
                dispatcherTimer.Stop();
            }
            /// <summary>
            /// 调用组件
            /// </summary>
            /// <param name="content">文本内容</param>
            /// <param name="point">出现位置</param>
            public void HelpText(string content, System.Windows.Point point)
            {
                Content = content;
                if (OnShow != null)
                {
                    OnShow(this, EventArgs.Empty);
                }
                else
                {
                    Margin = new Thickness { Top = point.Y, Left = point.X };
                    base.Background = Background;
                }
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(Continued)
                };
                timer.Tick += ResetText;
                timer.Start();
            }
            /// <summary>
            /// 带有颜色动画的提示框
            /// </summary>
            /// <param name="content">文本内容</param>
            /// <param name="point">出现位置</param>
            /// <param name="color">背景色的动画效果</param>
            public void HelpText(string content, System.Windows.Point point, ColorAnimation color)
            {
                Content = content;
                if (OnShow != null)
                {
                    OnShow(this, EventArgs.Empty);
                }
                else
                {
                    base.Background.BeginAnimation(SolidColorBrush.ColorProperty, color);
                }
                Margin = new Thickness { Top = point.Y, Left = point.X };
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(Continued)
                };
                timer.Tick += ResetText;
                timer.Start();
            }
        }
        /// <summary>
        /// 弹出提示窗口
        /// </summary>
        public class MyMessageBox
        {


            private static MyMessageBoxResult SelectdItem = MyMessageBoxResult.None;
            /// <summary>
            /// 弹出提示窗口
            /// </summary>
            /// <param name="Text">弹窗内容</param>
            /// <param name="TitleText">弹窗标题</param>
            /// <param name="myMessageBoxButton">按钮配置</param>
            /// <param name="ConfirmText">确认按钮信息</param>
            /// <param name="NoText">取消按钮信息</param>
            /// <param name="ButtonText">自定义按钮信息</param>
            /// <returns></returns>
            public static MyMessageBoxResult Show(string Text, string TitleText = "标题", MyMessageBoxButton myMessageBoxButton = MyMessageBoxButton.Confirm, string ConfirmText = "是", string NoText = "否", string ButtonText = "取消")
            {
                Window window = new Window
                {
                    FontSize = 16.0,
                    Height = 180.0,
                    Width = 300.0,
                    MaxHeight = 180.0,
                    MaxWidth = 300.0,
                    MinHeight = 180.0,
                    MinWidth = 300.0,
                    BorderThickness = new Thickness
                    {
                        Top = 0.0,
                        Bottom = 0.0,
                        Left = 0.0,
                        Right = 0.0
                    },
                    Title = TitleText,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    WindowStyle = WindowStyle.ToolWindow
                };
                Grid grid = new Grid();
                Canvas canvas = new Canvas
                {
                    Margin = new Thickness
                    {
                        Left = 0.0,
                        Top = 0.0,
                        Right = 0.0,
                        Bottom = 30.0
                    }
                };
                Canvas canvas2 = new Canvas
                {
                    Margin = new Thickness
                    {
                        Left = 0.0,
                        Top = 110.0,
                        Right = 0.0,
                        Bottom = 0.0
                    },
                    Background = Brushes.Gray
                };
                TextBlock textBlock = new TextBlock
                {
                    FontSize = 16.0,
                    TextAlignment = TextAlignment.Left,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness
                    {
                        Left = 10.0
                    },
                    MaxHeight = 108.0,
                    MaxWidth = 274.0
                };
                Button button = new Button
                {
                    Margin = new Thickness
                    {
                        Top = 5.0,
                        Left = 10.0
                    },
                    Name = "Confirm"
                };
                button.Click += Click;
                button.FontSize = 14.0;
                button.Tag = window;
                button.MinWidth = 50.0;
                Button button2 = new Button
                {
                    Margin = new Thickness
                    {
                        Top = 5.0,
                        Left = 115.0
                    },
                    Name = "No"
                };
                button2.Click += Click;
                button2.FontSize = 14.0;
                button2.Tag = window;
                button2.MinWidth = 50.0;
                Button button3 = new Button
                {
                    Margin = new Thickness
                    {
                        Top = 5.0,
                        Left = 215.0
                    },
                    Name = "Buttons"
                };
                button3.Click += Click;
                button3.FontSize = 14.0;
                button3.Tag = window;
                button3.MinWidth = 50.0;
                textBlock.Text = Text;
                button3.Content = ButtonText;
                button.Content = ConfirmText;
                button2.Content = NoText;
                switch (myMessageBoxButton)
                {
                    case MyMessageBoxButton.Confirm:
                        canvas2.Children.Add(button);
                        break;
                    case MyMessageBoxButton.ConfirmNO:
                        canvas2.Children.Add(button);
                        canvas2.Children.Add(button2);
                        break;
                }
                canvas.Children.Add(textBlock); ;
                grid.Children.Add(canvas);
                grid.Children.Add(canvas2);
                window.Content = grid;
                window.ShowDialog();
                new Task(delegate
                {
                    Thread.Sleep(200);
                    SelectdItem = MyMessageBoxResult.None;
                }).Start();
                return SelectdItem;
            }

            private static void Click(object o, RoutedEventArgs e)
            {
                Button button = (Button)o;
                switch (button.Name)
                {
                    case "Confirm":
                        SelectdItem = MyMessageBoxResult.Comfirm;
                        break;
                    case "No":
                        SelectdItem = MyMessageBoxResult.No;
                        break;
                    case "Buttons":
                        SelectdItem = MyMessageBoxResult.Buttons;
                        break;
                }
                ((Window)button.Tag).Close();
            }
        }
    }
    namespace CustomInputBox
    {
        /// <summary>
        /// 弹出用户输入信息窗口
        /// </summary>
        public class BaseInputBox : CustomComponent
        {
            private Result SelectdItem = Result.None;
            /// <summary>
            /// 获取返回值
            /// </summary>
            public string ReturnVaule = "";
            /// <summary>
            /// 窗口控件主模块
            /// </summary>
            public BaseControl ItemPad = new BaseControl();
            private IInputBoxCreate CustomCreate;
            /// <summary>
            /// 自定义模块
            /// </summary>
            /// <param name="box"></param>
            public virtual void Custom(IInputBoxCreate box)
            {
                CustomCreate = box;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="content"></param>
            /// <param name="title"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public override string Show(string content, string title, ButtonType type = ButtonType.Confirm)
            {
                Height = 150;
                Width = 268;
                ItemPad.Comfirm.Click += ButtonClick;
                ItemPad.No.Click += ButtonClick;
                Title = title;
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WindowStyle = WindowStyle.ToolWindow;
                if (CustomCreate != null)
                {
                    CustomCreate.Create(ItemPad);
                }

                Grid grid = new Grid();
                Label label = new Label
                {
                    Content = content,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = 0, Top = 5 },
                    VerticalAlignment = VerticalAlignment.Top
                };
                ItemPad.TextArea.Children.Add(label);
                switch (type)
                {
                    case ButtonType.Confirm:
                        ItemPad.No.Visibility = Visibility.Collapsed;
                        break;
                }
                grid.Children.Add(ItemPad);
                Content = grid;
                base.ShowDialog();
                if (SelectdItem == Result.No)
                {
                    return "";
                }
                else
                {
                    return ReturnVaule;
                }
            }
            private void ButtonClick(object sender, RoutedEventArgs e)
            {
                Button buttons = (Button)sender;
                switch (buttons.Name)
                {
                    case "yes":
                        SelectdItem = Result.Comfirm;
                        break;
                    case "no":
                        SelectdItem = Result.No;
                        break;
                }
                base.Close();
            }
        }
        /// <summary>
        /// 文本输入框
        /// </summary>
        public class InputBox : BaseInputBox, IInputBoxCreate
        {
            /// <summary>
            /// 弹出窗口
            /// </summary>
            /// <param name="content">提示文本</param>
            /// <param name="title">标题</param>
            /// <param name="type">按钮类型</param>
            /// <returns></returns>
            public override string Show(string content, string title, ButtonType type = ButtonType.Confirm)
            {
                Custom(this);
                return base.Show(content, title, type);
            }
            /// <summary>
            /// 创建主要控件
            /// </summary>
            /// <param name="ItemPad"></param>
            public virtual void Create(BaseControl ItemPad)
            {
                TextBox text = new TextBox
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = 10, Top = 5 },
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 20,
                    Width = 235,
                    TextWrapping = TextWrapping.Wrap
                };
                text.TextChanged += Text_TextChanged;
                ItemPad.Mainbox.Children.Add(text);
            }
            private void Text_TextChanged(object sender, TextChangedEventArgs e)
            {
                TextBox text = sender as TextBox;
                ReturnVaule = text.Text;
            }
        }
        /// <summary>
        /// 日期输入框
        /// </summary>
        public class DateInputBox : BaseInputBox, IInputBoxCreate
        {
            /// <summary>
            /// 弹出窗口
            /// </summary>
            /// <param name="content">提示文本</param>
            /// <param name="title">标题</param>
            /// <param name="type">按钮类型</param>
            /// <returns></returns>
            public override string Show(string content, string title, ButtonType type = ButtonType.Confirm)
            {
                Custom(this);
                return base.Show(content, title, type);
            }
            /// <summary>
            /// 创建主要控件
            /// </summary>
            /// <param name="control"></param>
            public virtual void Create(BaseControl control)
            {
                DatePicker picker = new DatePicker
                {
                    Text = "请选择日期：",
                    Width = 100,
                    Height = 20,
                    Margin = new Thickness { Left = 10, Top = 5 },
                };
                picker.MouseLeave += Picker_MouseLeave;
                ItemPad.Mainbox.Children.Add(picker);
            }

            private void Picker_MouseLeave(object sender, EventArgs e)
            {
                DatePicker picker = (DatePicker)sender;
                ReturnVaule = picker.SelectedDate.ToString();
            }
        }
    }
    namespace InPutBoxfoundation
    {
        /// <summary>
        /// 用于创建自定义弹窗的抽象类
        /// </summary>
        public abstract class CustomComponent : Window
        {
            /// <summary>
            /// 弹出提示窗口
            /// </summary>
            /// <param name="Text">弹窗内容</param>
            /// <param name="TitleText">弹窗标题</param>
            /// <param name="myMessageBoxButton">按钮配置</param>
            /// <returns></returns>
            public abstract string Show(string Text, string TitleText, ButtonType myMessageBoxButton = ButtonType.Confirm);
            /// <summary>
            /// 弹出窗口的按钮选择
            /// </summary>
            public enum ButtonType
            {/// <summary>
             /// 显示确认与取消按钮
             /// </summary>
                ConfirmNO,
                /// <summary>
                /// 显示确认取消与自定义按钮
                /// </summary>
                Confirm
            }
            /// <summary>
            /// 弹窗返回的用户选择
            /// </summary>
            public enum Result
            {
                /// <summary>
                /// 用户点击了确认
                /// </summary>
                Comfirm,
                /// <summary>
                /// 用户点击了取消
                /// </summary>
                No,
                /// <summary>
                /// 用户点击了自定义按钮
                /// </summary>
                None
            }
        }

        /// <summary>
        /// 自定义输入窗口的接口
        /// </summary>
        public interface IInputBoxCreate
        {
            void Create(BaseControl control);
        }
    }



}
