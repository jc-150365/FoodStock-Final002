using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FoodStock01
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryPage1 : ContentPage
    {
        /***ここからフィールド***/
        DateTime yyyymmdd; //フードピッカーの値を一時的に保持する

        TimeSpan s; 

        DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);//現在日付

        int result;

        bool s_switch = false;//食材と保存どちらのインサートメソッドを呼び出すかのやつ

        int qty = 0;
        /***ここまでフィールド***/

        public EntryPage1(string title)
        {
            //タイトル
            Title = title;

            //アイコン
            Icon = "plus32.png";

            //初期化
            InitializeComponent();

            string imagename = Device.Idiom.ToString();
            if (imagename.Equals("Phone")) //iphoneのとき
            {
                image.Source = "image_iphone.jpeg";
            }
            else //ipadのとき
            {
                image.Source = "image_ipad.jpeg";
            }
        }

        void SelectSwitch(object sender, ToggledEventArgs args)
        {
            //保存食品
            if (args.Value)
            {
                FoodPicker.IsEnabled = false;
                NumEntry.IsEnabled = true;
                UnitEntry.IsEnabled = true;

                s_switch = true;//保存食品用のインサートを使う
            }
            //食材
            else
            {
                NumEntry.IsEnabled = false;
                UnitEntry.IsEnabled = false;
                FoodPicker.IsEnabled = true;

                s_switch = false;//食材用のインサートを使う
            }
        }

        /***************「登録ボタン」が押された時*********************/
        private void Insert01_Clicked(object sender, EventArgs e)
        {
            if (!(NameEntry.Text==null) && !(NameEntry.Text.Equals("")))
            {
                if (!s_switch)//食材の登録だったら
                {
                    if (!(yyyymmdd.ToString("yyyy/MM/dd").Equals("0001/01/01")))//日付が入力されている
                    {
                        FoodModel.InsertFood(1, NameEntry.Text, result, yyyymmdd);//試し
                        DisplayAlert(NameEntry.Text + yyyymmdd.ToString("yyyy/MM/dd"), "あと" + result.ToString() + "日", "OK");

                        NameEntry.Text = "";

                        FoodPicker.Date = new DateTime(now.Year, now.Month, now.Day);
                    }
                    else
                    {
                        yyyymmdd = now;
                        FoodModel.InsertFood(1, NameEntry.Text, result, yyyymmdd);//試し
                        DisplayAlert(NameEntry.Text + yyyymmdd.ToString("yyyy/MM/dd"), "あと" + result.ToString() + "日", "OK");

                        NameEntry.Text = "";

                        FoodPicker.Date = new DateTime(now.Year, now.Month, now.Day);
                    }
                }
                else//保存食品の登録だったら
                {
                    if ((!(NumEntry.Text == null) && !(NumEntry.Text.Equals(""))) && (!(UnitEntry.Text == null) && !(UnitEntry.Text.Equals("")))) {
                        if (!(yyyymmdd.ToString("yyyy/MM/dd").Equals("0001/01/01")))//日付が入力されている
                        {
                            qty = int.Parse(NumEntry.Text);
                            StockFoodModel.InsertStock(1, NameEntry.Text, qty, UnitEntry.Text);
                            DisplayAlert(NameEntry.Text, qty.ToString() + UnitEntry.Text, "OK");

                            NameEntry.Text = "";
                            NumEntry.Text = "";
                            UnitEntry.Text = "";
                        }
                        else
                        {
                            yyyymmdd = now;
                            qty = int.Parse(NumEntry.Text);
                            StockFoodModel.InsertStock(1, NameEntry.Text, qty, UnitEntry.Text);
                            DisplayAlert(NameEntry.Text, qty.ToString() + UnitEntry.Text, "OK");

                            NameEntry.Text = "";
                            NumEntry.Text = "";
                            UnitEntry.Text = "";
                        }
                    }
                    else
                    {
                        DisplayAlert("入力エラー", "数量・単位を入力してください", "OK");
                    }
                }
            }
            else
            {
                DisplayAlert("入力エラー", "食品名を入力してください", "OK");
            }

        }
        
        /*************フードピッカーで日付を選択したとき******************/
        private void FoodPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            yyyymmdd = new DateTime(FoodPicker.Date.Year, FoodPicker.Date.Month, FoodPicker.Date.Day);

            s = yyyymmdd - now;

            result = s.Days;
        }

        /*************日付関係の試し*********************************************/
        public static int Span(DateTime d)
        {
            TimeSpan t = d - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            int span = t.Days;

            return span;
        }
    }
}