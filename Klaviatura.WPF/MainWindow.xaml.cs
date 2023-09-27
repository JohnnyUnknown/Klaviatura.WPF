using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Klaviatura.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        Stopwatch time = new Stopwatch();
        int charsCount = 0;     // Счетчик введенных знаков
        int failCount = 0;      // Счетчик ошибок ввода
        bool metkaStart = false, metkaStop = false;     // Для ограничения повторного нажатия кнопок
        SolidColorBrush color;
        int setNames = 1;       // Символ регистра
        string textInput;       // Временная строка ввода

        public MainWindow()
        {
            InitializeComponent();
        }

        // Старт работы программы
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (!metkaStart)
            {
                metkaStart = true;
                metkaStop = false;

                // Сброс параметров, измененных ранее
                textSpeed.Text = "0";
                textFails.Text = "0";
                textLineInput.Clear();
                // Изменение цвета фона текста
                textLine.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
                textLineInput.Background = new System.Windows.Media.SolidColorBrush(Colors.LightGray);

                // Формирование набора возможных знаков исходя из настроек программы
                string chars;
                if (sliderDiff.Value == 1)
                {
                    if (checkCase.IsChecked == false)
                        chars = "abcdefghijklmnopqrstuvwxyz     ";
                    else
                        chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz     ";
                }
                else if (sliderDiff.Value == 2)
                {
                    if (checkCase.IsChecked == false)
                        chars = "abcdefghijklmnopqrstuvwxyz0123456789       ";
                    else
                        chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789       ";
                }
                // Количество букв и цифр удваивается, для сохранения баланса со знаками в строке
                else if (sliderDiff.Value == 3)
                {
                    if (checkCase.IsChecked == false)
                        chars = "abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789!@%*();,.:'\"<>?-_=+              ";
                    else
                        chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890123456789!@%*();,.:'\"<>?-_=+              ";
                }
                else
                {
                    if (checkCase.IsChecked == false)
                        chars = "abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()[];',./\\{}:\"|<>?-_=+`~               ";
                    else
                        chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890123456789!@#$%^&*()[];',./\\{}:\"|<>?-_=+`~               ";
                }

                // Начало отсчета времени секундомера
                time = Stopwatch.StartNew();

                // Формирование строки на 60 символов из определенного набора с удалением возможных пробелов в начале и конце строки
                textLine.Text = new string(Enumerable.Repeat(chars, 60).Select(s => s[random.Next(chars.Length)]).ToArray()).Trim();
            }

            // Установка фокуса на поле ввода текста
            textLineInput.Focus();
            // Установка разрешения на ввод текста в поле для ввода
            textLineInput.IsReadOnly = false;
        }

        // Остановка программы
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (!metkaStop)
            {
                metkaStop = true;
                metkaStart = false;
                // Остановка секундомера
                time.Stop();

                charsCount = textLineInput.Text.Length;

                failCount = 0;
                for (int i = 0; i < charsCount; i++)
                {
                    if (textLineInput.Text[i] != textLine.Text[i])
                    {
                        failCount++;
                    }
                }

                textFails.Text = failCount.ToString();
                textLineInput.Background = new System.Windows.Media.SolidColorBrush(Colors.LightSkyBlue);
                textLine.Background = new System.Windows.Media.SolidColorBrush(Colors.LightSkyBlue);

                Speed();

                // Обнуления счетчика введенных знаков
                charsCount = 0;
                // Установка запрета на ввод текста в поле для ввода
                textLineInput.IsReadOnly = true;
                // Очистка временной строки ввода
                textInput = null;
            }
        }

        // Определение скорости ввода символов пользователем
        private void Speed()
        {
            long t = time.ElapsedMilliseconds;
            double t1 = (double)(t / 1000);
            // Если пройденное время меньше 1 сек
            if (t1 >= 1)
            {
                // Вычисление скорости ввода символов пользователем
                t1 = charsCount / t1 * 60;
                t1 = Math.Round(t1, 1);
                textSpeed.Text = t1.ToString() + " chars/min";
            }
            else
            {
                // Принимаем пройденное время за 1 сек
                textSpeed.Text = (charsCount * 60).ToString() + " chars/min";
            }
        }

        // Обработка события ввода текста
        private void textLineInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Конкатенация введенного символа к временной строке ввода
            textInput += e.Text;
            // Определение кол-ва введенных знаков
            charsCount = textInput.Length;

            if (charsCount > 0)
            {
                // Если последний введенный символ равен соответствующему символу в строке-образце
                if (textInput[charsCount - 1] == textLine.Text[charsCount - 1])
                {
                    textLineInput.Background = new System.Windows.Media.SolidColorBrush(Colors.LightGreen);
                    textLine.Background = new System.Windows.Media.SolidColorBrush(Colors.LightGreen);
                }
                // Если последний введенный символ не равен соответствующему символу в строке-образце
                else if (textInput[charsCount - 1] != textLine.Text[charsCount - 1])
                {
                    // Увеличивается и выводится на экран кол-во ошибок и выводится на экран
                    failCount++;
                    textFails.Text = failCount.ToString();
                    textLineInput.Background = new System.Windows.Media.SolidColorBrush(Colors.Salmon);
                    textLine.Background = new System.Windows.Media.SolidColorBrush(Colors.Salmon);
                }

                Speed();
            }
        }

        // Описание события нажатия кнопок
        private void textLineInput_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Обработка нажатия клавиш Delete и Back
            if (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back)
            {
                charsCount--;
                Speed();
            }
            // Обработка нажатия клавиш Shift и CapsLock
            else if (e.Key == System.Windows.Input.Key.LeftShift || e.Key == System.Windows.Input.Key.RightShift || e.Key == System.Windows.Input.Key.CapsLock)
            {
                if (setNames == 1)
                    ChangeNamesButtonsUp();
                else if (setNames == 2)
                    ChangeNamesButtonsDown();

                // Изменение символа регистра для CapsLock
                if (e.Key == System.Windows.Input.Key.CapsLock) setNames = (setNames == 1) ? 2 : 1;
            }


            if (e.Key == System.Windows.Input.Key.OemTilde)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonTilda.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D1)
            {
                color = new SolidColorBrush(Colors.Salmon);
                button1.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D2)
            {
                color = new SolidColorBrush(Colors.Salmon);
                button2.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D3)
            {
                color = new SolidColorBrush(Colors.Gold);
                button3.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D4)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                button4.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D5)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                button5.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D6)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                button6.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D7)
            {
                color = new SolidColorBrush(Colors.Violet);
                button7.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D8)
            {
                color = new SolidColorBrush(Colors.Violet);
                button8.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D9)
            {
                color = new SolidColorBrush(Colors.Salmon);
                button9.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D0)
            {
                color = new SolidColorBrush(Colors.Gold);
                button0.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemMinus)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonMinus.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemPlus)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonEqually.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.Back)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonBackspace.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }


            else if (e.Key == System.Windows.Input.Key.Tab)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonTab.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.Q)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonQ.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.W)
            {
                color = new SolidColorBrush(Colors.Gold);
                buttonW.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.E)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonE.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.R)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                buttonR.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.T)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                buttonT.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.Y)
            {
                color = new SolidColorBrush(Colors.Violet);
                buttonY.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.U)
            {
                color = new SolidColorBrush(Colors.Violet);
                buttonU.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.I)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonI.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.O)
            {
                color = new SolidColorBrush(Colors.Gold);
                buttonO.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.P)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonP.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemOpenBrackets)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonSquareBracketLeft.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemCloseBrackets)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonSquareBracketRight.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemBackslash)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonRevSlash.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }


            else if (e.Key == System.Windows.Input.Key.CapsLock)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonCapsLock.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.A)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonA.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.S)
            {
                color = new SolidColorBrush(Colors.Gold);
                buttonS.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.D)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonD.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.F)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                buttonF.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.G)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                buttonG.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.H)
            {
                color = new SolidColorBrush(Colors.Violet);
                buttonH.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.J)
            {
                color = new SolidColorBrush(Colors.Violet);
                buttonJ.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.K)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonK.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.L)
            {
                color = new SolidColorBrush(Colors.Gold);
                buttonL.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemSemicolon)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonSemicolon.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemQuotes)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonMarks.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonEnter.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }


            else if (e.Key == System.Windows.Input.Key.LeftShift)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonShift.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.Z)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonZ.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.X)
            {
                color = new SolidColorBrush(Colors.Gold);
                buttonX.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.C)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonC.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.V)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                buttonV.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.B)
            {
                color = new SolidColorBrush(Colors.LightSkyBlue);
                buttonB.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.N)
            {
                color = new SolidColorBrush(Colors.Violet);
                buttonN.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.M)
            {
                color = new SolidColorBrush(Colors.Violet);
                buttonM.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemComma)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonComma.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemPeriod)
            {
                color = new SolidColorBrush(Colors.Salmon);
                buttonDot.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.OemQuestion)
            {
                color = new SolidColorBrush(Colors.LightGreen);
                buttonSlash.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.RightShift)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonRShift.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }


            else if (e.Key == System.Windows.Input.Key.LeftCtrl)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonCtrl.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.LWin)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonWin.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.LeftAlt)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonAlt.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.Space)
            {
                color = new SolidColorBrush(Colors.DarkGoldenrod);
                buttonSpace.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);

                // Добавление пробела в введенную строку проверки (через событие не добавляется)
                charsCount++;
                textInput += " ";
            }
            else if (e.Key == System.Windows.Input.Key.RightAlt)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonRAlt.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.RWin)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonRWin.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }
            else if (e.Key == System.Windows.Input.Key.RightCtrl)
            {
                color = new SolidColorBrush(Colors.Gray);
                buttonRCtrl.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }


        }

        // Описание события отжима кнопок
        private void textLineInput_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Обработка отжатия клавиш Shift
            if (e.Key == System.Windows.Input.Key.LeftShift || e.Key == System.Windows.Input.Key.RightShift)
            {
                if (setNames == 1)
                    ChangeNamesButtonsDown();
                else if (setNames == 2)
                    ChangeNamesButtonsUp();
            }


            if (e.Key == System.Windows.Input.Key.OemTilde)
            {
                buttonTilda.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D1)
            {
                button1.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D2)
            {
                button2.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D3)
            {
                button3.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D4)
            {
                button4.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D5)
            {
                button5.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D6)
            {
                button6.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D7)
            {
                button7.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D8)
            {
                button8.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D9)
            {
                button9.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D0)
            {
                button0.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemMinus)
            {
                buttonMinus.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemPlus)
            {
                buttonEqually.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.Back)
            {
                buttonBackspace.Background = color;

                // Удаление последнего символа из введенной строки проверки
                if (charsCount >= 0)
                    textInput = textInput.Remove(charsCount, 1);
            }


            else if (e.Key == System.Windows.Input.Key.Tab)
            {
                buttonTab.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.Q)
            {
                buttonQ.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.W)
            {
                buttonW.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.E)
            {
                buttonE.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.R)
            {
                buttonR.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.T)
            {
                buttonT.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.Y)
            {
                buttonY.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.U)
            {
                buttonU.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.I)
            {
                buttonI.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.O)
            {
                buttonO.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.P)
            {
                buttonP.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemOpenBrackets)
            {
                buttonSquareBracketLeft.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemCloseBrackets)
            {
                buttonSquareBracketRight.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemBackslash)
            {
                buttonRevSlash.Background = new System.Windows.Media.SolidColorBrush(Colors.LightBlue);
            }


            else if (e.Key == System.Windows.Input.Key.CapsLock)
            {
                buttonCapsLock.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.A)
            {
                buttonA.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.S)
            {
                buttonS.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.D)
            {
                buttonD.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.F)
            {
                buttonF.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.G)
            {
                buttonG.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.H)
            {
                buttonH.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.J)
            {
                buttonJ.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.K)
            {
                buttonK.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.L)
            {
                buttonL.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemSemicolon)
            {
                buttonSemicolon.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemQuotes)
            {
                buttonMarks.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                buttonEnter.Background = color;
            }


            else if (e.Key == System.Windows.Input.Key.LeftShift)
            {
                buttonShift.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.Z)
            {
                buttonZ.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.X)
            {
                buttonX.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.C)
            {
                buttonC.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.V)
            {
                buttonV.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.B)
            {
                buttonB.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.N)
            {
                buttonN.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.M)
            {
                buttonM.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemComma)
            {
                buttonComma.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemPeriod)
            {
                buttonDot.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.OemQuestion)
            {
                buttonSlash.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.RightShift)
            {
                buttonRShift.Background = color;
            }


            else if (e.Key == System.Windows.Input.Key.LeftCtrl)
            {
                buttonCtrl.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.LWin)
            {
                buttonWin.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.LeftAlt)
            {
                buttonAlt.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.Space)
            {
                buttonSpace.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.RightAlt)
            {
                buttonRAlt.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.RWin)
            {
                buttonRWin.Background = color;
            }
            else if (e.Key == System.Windows.Input.Key.RightCtrl)
            {
                buttonRCtrl.Background = color;
            }
        }

        // Метод изменения представления для кнопок 
        private void ChangeNamesButtonsUp()
        {
            buttonTilda.Content = "~";
            button1.Content = "!";
            button2.Content = "@";
            button3.Content = "#";
            button4.Content = "$";
            button5.Content = "%";
            button6.Content = "^";
            button7.Content = "&";
            button8.Content = "*";
            button9.Content = "(";
            button0.Content = ")";
            buttonMinus.Content = "_";
            buttonEqually.Content = "+";
            buttonQ.Content = "Q";
            buttonW.Content = "W";
            buttonE.Content = "E";
            buttonR.Content = "R";
            buttonT.Content = "T";
            buttonY.Content = "Y";
            buttonU.Content = "U";
            buttonI.Content = "I";
            buttonO.Content = "O";
            buttonP.Content = "P";
            buttonSquareBracketLeft.Content = "{";
            buttonSquareBracketRight.Content = "}";
            buttonRevSlash.Content = "|";
            buttonA.Content = "A";
            buttonS.Content = "S";
            buttonD.Content = "D";
            buttonF.Content = "F";
            buttonG.Content = "G";
            buttonH.Content = "H";
            buttonJ.Content = "J";
            buttonK.Content = "K";
            buttonL.Content = "L";
            buttonSemicolon.Content = ":";
            buttonMarks.Content = "\"";
            buttonZ.Content = "Z";
            buttonX.Content = "X";
            buttonC.Content = "C";
            buttonV.Content = "V";
            buttonB.Content = "B";
            buttonN.Content = "N";
            buttonM.Content = "M";
            buttonComma.Content = "<";
            buttonDot.Content = ">";
            buttonSlash.Content = "?";

        }

        // Метод изменения представления для кнопок 
        private void ChangeNamesButtonsDown()
        {
            buttonTilda.Content = "`";
            button1.Content = "1";
            button2.Content = "2";
            button3.Content = "3";
            button4.Content = "4";
            button5.Content = "5";
            button6.Content = "6";
            button7.Content = "7";
            button8.Content = "8";
            button9.Content = "9";
            button0.Content = "0";
            buttonMinus.Content = "-";
            buttonEqually.Content = "=";
            buttonQ.Content = "q";
            buttonW.Content = "w";
            buttonE.Content = "e";
            buttonR.Content = "r";
            buttonT.Content = "t";
            buttonY.Content = "y";
            buttonU.Content = "u";
            buttonI.Content = "i";
            buttonO.Content = "o";
            buttonP.Content = "p";
            buttonSquareBracketLeft.Content = "[";
            buttonSquareBracketRight.Content = "]";
            buttonRevSlash.Content = "\\";
            buttonA.Content = "a";
            buttonS.Content = "s";
            buttonD.Content = "d";
            buttonF.Content = "f";
            buttonG.Content = "g";
            buttonH.Content = "h";
            buttonJ.Content = "j";
            buttonK.Content = "k";
            buttonL.Content = "l";
            buttonSemicolon.Content = ";";
            buttonMarks.Content = "'";
            buttonZ.Content = "z";
            buttonX.Content = "x";
            buttonC.Content = "c";
            buttonV.Content = "v";
            buttonB.Content = "b";
            buttonN.Content = "n";
            buttonM.Content = "m";
            buttonComma.Content = ",";
            buttonDot.Content = ".";
            buttonSlash.Content = "/";
        }

        // Изменение и отображение уровня сложности
        private void sliderDiff_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelDifficulty.Content = sliderDiff.Value.ToString();
        }
    }
}
