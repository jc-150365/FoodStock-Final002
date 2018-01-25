﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FoodStock01;
using System.Windows.Input;

namespace FoodStock01
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FoodPage1 : ContentPage
    {
        String s = "http://cookpad.com/search/";

        ContentView qsl; // iOS/Android 用
        ContentView bl; // 黒背景
        bool qslVisible = true;

        public FoodPage1(string title)
        {
            //タイトル
            Title = title;

            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem
            {
                Text = "検索",
                Icon = "search24.png",
                Command = new Command(() =>
                    //ページ遷移
                    Navigation.PushAsync(new NextPage(s))
                ),
            });


            ///***********************初回起動時に表示されます ここから**********************///
            AbsoluteLayout abs = new AbsoluteLayout { };

            // 上に表示される View です。
            // 黒背景を最初に置いて、
            bl = new ContentView
            {
                BackgroundColor = Color.Black,
                Opacity = 0.6
            };

            
            // 更に上に View を被せました。
            qsl = new ContentView
            {
                Content = new StackLayout
                {
                    Children = {
                        // 画像で誤魔化します
                        new Image {
                            Source = "start_search.png",
                        },
                        new Image {
                            Source = "start_refresh.png",
                        },
                         new Image {
                            Source = "start_tab.png",
                            VerticalOptions = LayoutOptions.EndAndExpand,
                        },
                        // 閉じると Application Properties に bool 値を保存します。
                        new Button {
                            Text = "閉じる",
                            TextColor = Color.White,
                            BackgroundColor = Color.FromHex("1e90ff"),
                            VerticalOptions = LayoutOptions.EndAndExpand,
                            Command = new Command (()=>{
                                qsl.IsVisible = false;
                                bl.IsVisible = false;
                                qslVisible = false;
                                Application.Current.Properties["qsl"] = qslVisible;

                                //画面再読み込み
                                Title = "食材リスト";
                                s = "http://cookpad.com/search/";
                                InitializeComponent();

                            }),
                        },
                    }
                }
            };
            
            ///******************************** ここまで ************************************///


            // AbsoluteLayout にコントロールを追加しますがその際に
            // Properties Dictionary をチェックして QuickStart Layer を追加しています。
            abs.Children.Add(ml);

            //keyが格納されているか
            if (Application.Current.Properties.ContainsKey("qsl"))
            {
                //初回true
                var bqsl = (bool)Application.Current.Properties["qsl"];
                if (bqsl)
                {
                    abs.Children.Add(bl);
                    abs.Children.Add(qsl);
                    Content = abs;
                }//falseが格納されている(2回目以降)
                else
                {
                    Title = "食材リスト";
                    s = "http://cookpad.com/search/";

                    InitializeComponent();
                }
                    
            }
            else
            {
                abs.Children.Add(bl);
                abs.Children.Add(qsl);
                Content = abs;
            }
            SizeChanged += OnPageSizeChanged;
        }
        
        /// <summary>
        /// 画面サイズ変更時(起動して画面が表示される時)に呼び出されます。この後でないと画面サイズが取得できません。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnPageSizeChanged(object sender, EventArgs args)
        {
            var w = this.Width;
            var h = this.Height;

            /*AbsoluteLayout.SetLayoutFlags(ml, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(ml, new Rectangle(10, 10, w-20, h));*/

            AbsoluteLayout.SetLayoutFlags(bl, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(bl, new Rectangle(0d, 0d, w, h));

            AbsoluteLayout.SetLayoutFlags(qsl, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(qsl, new Rectangle(0d, 0d, w, h));
        }
        

        void ChackBoxChanged(object sender, bool isChecked)
        {
            //選択された時の処理
            if (isChecked)
            {
                s += ((CheckBox)sender).Text + "　";
            }
            //選択が外された時の処理
            else
            {
                s = s.Replace(((CheckBox)sender).Text + "　", "");
            }
        }

        //デリート押された
        //void OnDelete_Clicked(object sender, EventArgs args)
        async void OnDelete_Clicked(object sender, EventArgs args)
        {
            string no = ((CustomButtonDelete)sender).NoText;
            string name = ((CustomButtonDelete)sender).NameText;

            var result = await DisplayAlert("削除", "この食材を削除しますか", "OK", "キャンセル");
            if (result == true)
            {
                int f_no = int.Parse(no);
                FoodModel.DeleteFood(f_no);

                Title = "食材リスト";
                s = "http://cookpad.com/search/";

                InitializeComponent();
            }
        }

        //初回起動時の状態にします（テスト用）
        private void Set_Start_Clicked(object sender, EventArgs e)
        {
            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day -1);
            Application.Current.Properties["date"] = d;
        }

        //読み込まれたとき
        protected override void OnAppearing()
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string todays = today.ToString("yyyy/MM/dd");

            //keyが格納されているか
            if (Application.Current.Properties.ContainsKey("date"))
            {
                DateTime lastdate = (DateTime)Application.Current.Properties["date"];

                //今日と保存されている日付を比較
                if (!(lastdate == today))
                {
                    //今日まだ
                    Application.Current.Properties["date"] = today;
                    //期限付き
                    if (FoodModel.SelectF_result() != -999 && FoodModel.SelectF_result() > 0)
                    {
                        DisplayAlert("期限通知", "期限まであと" + SettingModel.SelectSetting_Max().ToString() + "日の食材があります", "OK");
                    }
                }
                
            }
            else//はじめて
            {
                Application.Current.Properties["date"] = today;
                if (FoodModel.SelectF_result() != -999 && FoodModel.SelectF_result() > 0)
                {
                    DisplayAlert("期限通知", "期限まであと" + SettingModel.SelectSetting_Max().ToString() + "日の食材があります", "OK");
                }
            }
        }

        //引っ張ったとき（更新）
        private async void OnRefreshing(object sender, EventArgs e)
        {
            // 1秒処理を待つ
            await Task.Delay(1000);

            //リフレッシュを止める
            list.IsRefreshing = false;

            Title = "食材リスト";
            s = "http://cookpad.com/search/";

            InitializeComponent();
        }
    }
}