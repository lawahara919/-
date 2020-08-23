using System;
using System.Windows.Forms;

namespace KawaharaSoftware.EasyCalc
{
    
    public partial class FormEasyCalc : Form
    {
        // ********** フィールド **********

        #region 定数

        /// <summary>
        /// 定数 - 表示の初期値(0)
        /// </summary>
        private const string ZERO = "0";

        /// <summary>
        /// 定数 - 小数点
        /// </summary>
        private const string DECIMAL_POINT = ".";

        #endregion

        #region 列挙体

        /// <summary>
        /// 列挙体 - 現在の状況
        /// </summary>
        private enum NOW_STATUS
        {
            /// <summary>
            /// 左辺入力中
            /// </summary>
            INPUT_LEFT_SIDE = 0,
            /// <summary>
            /// 記号入力
            /// </summary>
            INPUT_SYMBOL = 1,
            /// <summary>
            /// 右辺入力中
            /// </summary>
            INPUT_RIGHT_SIDE = 2,
            /// <summary>
            /// 結果表示中
            /// </summary>
            SHOW_RESULT = 3
        }

        /// <summary>
        /// 列挙体 - ＋－×÷指定状況
        /// </summary>
        private enum CLICK_SYMBOL
        {
            /// <summary>
            /// 未指定
            /// </summary>
            NONE = 0,
            /// <summary>
            /// ＋
            /// </summary>
            ADD = 1,
            /// <summary>
            /// －
            /// </summary>
            SUBTRACT = 2,
            /// <summary>
            /// ×
            /// </summary>
            MULTIPLY = 3,
            /// <summary>
            /// ÷
            /// </summary>
            DIVIDE = 4
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ＋－×÷指定状況（対応するボタンが押されたら更新される）
        /// </summary>
        private CLICK_SYMBOL ClickSymbol = CLICK_SYMBOL.NONE;

        /// <summary>
        /// 現在の状況（左辺入力中→記号入力→右辺入力中→結果表示の順に遷移する）
        /// </summary>
        private NOW_STATUS NowStatus = NOW_STATUS.INPUT_LEFT_SIDE;

        /// <summary>
        /// 左辺に入力された値（確定値）
        /// </summary>
        private decimal LeftSideValue = 0;

        /// <summary>
        /// 右辺に入力された値（確定値）
        /// </summary>
        private decimal RightSideValue = 0;

        #endregion

        // ********** コンストラクタ **********

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormEasyCalc()
        {
            InitializeComponent();
        }
        #endregion

        // ********** フォームロード **********

        #region フォームロード（画面が最初に表示される際に発生する）
        /// <summary>
        /// フォームロード（画面が最初に表示される際に発生する）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEasyCalc_Load(object sender, EventArgs e)
        {
            //初期化
            Reset();

            //0～9までのボタンのイベントは、すべてbtnX_Click()メソッドで処理する（処理内容がほぼ同じのため、統合）
            btn0.Click += new EventHandler(btnX_Click);
            btn1.Click += new EventHandler(btnX_Click);
            btn2.Click += new EventHandler(btnX_Click);
            btn3.Click += new EventHandler(btnX_Click);
            btn4.Click += new EventHandler(btnX_Click);
            btn5.Click += new EventHandler(btnX_Click);
            btn6.Click += new EventHandler(btnX_Click);
            btn7.Click += new EventHandler(btnX_Click);
            btn8.Click += new EventHandler(btnX_Click);
            btn9.Click += new EventHandler(btnX_Click);
        }
        #endregion

        // ********** キーボード入力イベント **********

        #region キーボードの何らかのキーがが押下された場合のイベント受け元
        /// <summary>
        /// キーボードの何らかのキーがが押下された場合のイベント受け元
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEasyCalc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //0～9までのキーが押された場合
                if (InputNumeric(e)) return;

                //＋－×÷などのキーが押された場合
                if (InputSymbol(e)) return;

                //Ctrl+Cが押された場合、現在表示されている数字をクリップボードにコピーする
                if (e.KeyData == (Keys.Control | Keys.C))
                {
                    Clipboard.SetText(lblInputValue.Text);
                }
                //Ctrl+Vが押された場合、現在表示されている数字をクリップボードにコピーする
                else if (e.KeyData == (Keys.Control | Keys.V))
                {
                    //クリップボードに保存されているデータが文字列の場合
                    if (Clipboard.ContainsText())
                    {
                        //クリップボードに保存されている文字列を、画面に表示する
                        Show3DigitSeparator(Clipboard.GetText());
                    }
                }

                //それ以外のキーが押された場合は、対応する処理がないので無視
            }
            catch (Exception ex)
            {
                MessageBox.Show("キー押下時に例外エラー発生。エラー:" + ex.ToString(), "エラー");
            }
        }
        #endregion

        #region 0～9までのキーが押された場合、対応する数字を画面に表示する
        /// <summary>
        /// 0～9までのキーが押された場合、対応する数字を画面に表示する
        /// 
        /// 数字のボタンを押した際と処理が全く同じになるため、
        /// ボタンのPerformClick()メソッドを呼び出すことで入力を実現している。
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true:該当イベントが存在した、false:該当イベント存在せず</returns>
        private bool InputNumeric(KeyEventArgs e)
        {
            //押されたキーによって分岐
            switch (e.KeyCode)
            {
                //0が押された
                case Keys.D0:       //0キー
                case Keys.NumPad0:  //テンキーの0キー
                    btn0.PerformClick();    //0ボタンを押す
                    return true;    //該当するボタンが存在した

                //1が押された
                case Keys.D1:       //1キー
                case Keys.NumPad1:  //テンキーの1キー
                    btn1.PerformClick();    //1ボタンを押す
                    return true;    //該当するボタンが存在した

                //2が押された
                case Keys.D2:       //2キー
                case Keys.NumPad2:  //テンキーの2キー
                    btn2.PerformClick();    //2ボタンを押す
                    return true;    //該当するボタンが存在した

                //3が押された
                case Keys.D3:       //3キー
                case Keys.NumPad3:  //テンキーの3キー
                    btn3.PerformClick();    //3ボタンを押す
                    return true;    //該当するボタンが存在した

                //4が押された
                case Keys.D4:       //4キー
                case Keys.NumPad4:  //テンキーの4キー
                    btn4.PerformClick();    //4ボタンを押す
                    return true;    //該当するボタンが存在した

                //5が押された
                case Keys.D5:       //5キー
                case Keys.NumPad5:  //テンキーの5キー
                    btn5.PerformClick();    //5ボタンを押す
                    return true;    //該当するボタンが存在した

                //6が押された
                case Keys.D6:       //6キー
                case Keys.NumPad6:  //テンキーの6キー
                    btn6.PerformClick();    //6ボタンを押す
                    return true;    //該当するボタンが存在した

                //7が押された
                case Keys.D7:       //7キー
                case Keys.NumPad7:  //テンキーの7キー
                    btn7.PerformClick();    //7ボタンを押す
                    return true;    //該当するボタンが存在した

                //8が押された
                case Keys.D8:       //8キー
                case Keys.NumPad8:  //テンキーの8キー
                    btn8.PerformClick();    //8ボタンを押す
                    return true;    //該当するボタンが存在した

                //9が押された
                case Keys.D9:       //9キー
                case Keys.NumPad9:  //テンキーの9キー
                    btn9.PerformClick();    //9ボタンを押す
                    return true;    //該当するボタンが存在した
            }

            //該当するボタンが無かった
            return false;
        }
        #endregion

        #region ＋－×÷＝．のキーが押された場合、対応する処理を行う
        /// <summary>
        /// ＋－×÷＝．のキーが押された場合、対応する処理を行う
        /// 
        /// 各ボタンを押した際と処理が全く同じになるため、
        /// ボタンのPerformClick()メソッドを呼び出すことで実現している。
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool InputSymbol(KeyEventArgs e)
        {
            //押されたキーによって分岐
            switch (e.KeyCode)
            {
                //＋が押された
                case Keys.Add:
                    btnAdd.PerformClick();  //＋ボタンを押す
                    return true;    //該当するボタンが存在した

                //‐が押された
                case Keys.Subtract:
                    btnSubtract.PerformClick(); //‐ボタンを押す
                    return true;    //該当するボタンが存在した

                //＊(×)が押された
                case Keys.Multiply: 
                    btnMultiply.PerformClick();     //×ボタンを押す
                    return true;    //該当するボタンが存在した

                //／（÷）が押された
                case Keys.Divide: 
                    btnDivide.PerformClick();   //÷ボタンを押す
                    return true;    //該当するボタンが存在した

                //EnterまたはReturnが押された
                case Keys.Enter:
                    btnEqual.PerformClick(); //＝ボタンを押す
                    return true;    //該当するボタンが存在した

                //BackSpaceが押された
                case Keys.Back:
                    btnBackSpace.PerformClick(); //←ボタンを押す
                    return true;    //該当するボタンが存在した

                //Deleteが押された
                case Keys.Delete:
                    btnClearEnd.PerformClick(); //CEボタンを押す
                    return true;    //該当するボタンが存在した

                //.が押された
                case Keys.Decimal:
                case Keys.OemPeriod:
                    btnDecimalPoint.PerformClick(); //．ボタンを押す
                    return true;    //該当するボタンが存在した
            }

            //該当するボタンが無かった
            return false;
        }
        #endregion

        // ********** ボタンクリックイベント **********

        #region 0～9までのボタンクリックイベント
        /// <summary>
        /// 0～9までのボタンクリックイベント
        /// 
        /// 0,1,2,3,4,5,6,7,8,9までのそれぞれのボタンは、押されたボタンに該当する数字を入力するという
        /// 同じ役割を持つため、すべてこのメソッド内で処理する。
        /// 第1引数のsenderに、押されたボタン自身が格納されているため、それを、  ((Button)sender) のようにキャスト（型変換）
        /// してあげることで、押されたボタンの数字が取得できる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnX_Click(object sender, EventArgs e)
        {
            //クリックされたボタンからフォーカスを外す
            //（この状態でEnterキーが押下されると、同じボタンが連打されてしまうのを防ぐ）
            lblInputValue.Focus();

            //前回＋－×÷が押されていた場合、右辺入力を開始する
            if (NowStatus == NOW_STATUS.INPUT_SYMBOL)
            {
                //現在の状況を右辺入力中に変更する
                NowStatus = NOW_STATUS.INPUT_RIGHT_SIDE;
                //表示
                Show3DigitSeparator(ZERO);
            }

            //前回結果表示中の場合、左辺入力に戻る
            if (NowStatus == NOW_STATUS.SHOW_RESULT)
            {
                //プログラム起動時の初期状態に戻す
                Reset();
            }

            //すでに12桁入力されている場合は、これ以上入力できないように無視する
            //12桁の判定は、3桁区切りに利用している「,」、小数点の「.」、符号の「-」は除いて判断する。
            //（12桁にした理由は特に意味はない。ただ、あまり大きくしすぎるとdecimalで表現可能な桁数を超えるため、それ未満に収まるようにする）
            if (lblInputValue.Text.Replace(",", "").Replace(".", "").Replace("-", "").Length >= 12) return;

            //現在表示されているラベルの末尾に、押されたボタンに表示されているテキストを付けることで、
            //数字の追加入力を実現している
            Show3DigitSeparator(lblInputValue.Text + ((Button)sender).Text);
        }
        #endregion

        #region 【＋】ボタンクリックイベント
        /// <summary>
        /// 【＋】ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //＋ボタンが押されたことを登録する
            ClickSymbol = CLICK_SYMBOL.ADD;

            //記号が入力されたので、状態を左辺から記号入力に遷移させる
            NowStatus = NOW_STATUS.INPUT_SYMBOL;

            //現在入力されていた内容を左辺として登録する
            LeftSideValue = ConvertStringToDecimal(lblInputValue.Text);
        }
        #endregion

        #region 【－】ボタンクリックイベント
        /// <summary>
        /// 【－】ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubtract_Click(object sender, EventArgs e)
        {
            //－ボタンが押されたことを登録する
            ClickSymbol = CLICK_SYMBOL.SUBTRACT;

            //記号が入力されたので、状態を左辺から記号入力に遷移させる
            NowStatus = NOW_STATUS.INPUT_SYMBOL;

            //現在入力されていた内容を左辺として登録する
            LeftSideValue = ConvertStringToDecimal(lblInputValue.Text);
        }
        #endregion

        #region 【×】ボタンクリックイベント
        /// <summary>
        /// 【×】ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMultiply_Click(object sender, EventArgs e)
        {
            //×ボタンが押されたことを登録する
            ClickSymbol = CLICK_SYMBOL.MULTIPLY;

            //記号が入力されたので、状態を左辺から記号入力に遷移させる
            NowStatus = NOW_STATUS.INPUT_SYMBOL;

            //現在入力されていた内容を左辺として登録する
            LeftSideValue = ConvertStringToDecimal(lblInputValue.Text);
        }
        #endregion

        #region 【÷】ボタンクリックイベント
        /// <summary>
        /// 【÷】ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDivide_Click(object sender, EventArgs e)
        {
            //÷ボタンが押されたことを登録する
            ClickSymbol = CLICK_SYMBOL.DIVIDE;

            //記号が入力されたので、状態を左辺から記号入力に遷移させる
            NowStatus = NOW_STATUS.INPUT_SYMBOL;

            //現在入力されていた内容を左辺として登録する
            LeftSideValue = ConvertStringToDecimal(lblInputValue.Text);
        }
        #endregion

        #region 【＝】ボタンクリックイベント
        /// <summary>
        /// 【＝】ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEqual_Click(object sender, EventArgs e)
        {
            try
            {
                //＋－×÷の何れも押していない場合は、何もしない（計算できないため）
                if (ClickSymbol == CLICK_SYMBOL.NONE)
                {
                    return;
                }

                //表示されている文字をdecimalに変換する（計算できるように）
                decimal nowValue = ConvertStringToDecimal(lblInputValue.Text);

                //すでに結果表示中の場合に、再度 = がクリックされた場合、
                //表示されている値（前回の計算結果）を右辺に使うのではなく、最後に入力された右辺を利用する。
                //こうしないと、以下の場合に意図した結果にならない。
                //例． 100 + 1 = 101  をまず計算する。次にこの状態で = をクリックすると、 101 + 1 = 102としたい。
                //     こうするためには、計算終了時に左辺に計算結果を保存し、右辺に現在入力された値を保存しておく必要がある。
                if (NowStatus == NOW_STATUS.SHOW_RESULT)
                {
                    nowValue = RightSideValue;
                }

                //押された＋－×÷により、計算形式を変える
                switch (ClickSymbol)
                {
                    //＋
                    case CLICK_SYMBOL.ADD:
                        Show3DigitSeparator(LeftSideValue + nowValue);
                        break;

                    //－
                    case CLICK_SYMBOL.SUBTRACT:
                        Show3DigitSeparator(LeftSideValue - nowValue);
                        break;

                    //×（プログラム上では×は使えないので*を使う）
                    case CLICK_SYMBOL.MULTIPLY:
                        Show3DigitSeparator(LeftSideValue * nowValue);
                        break;

                    //÷（プログラム上では÷は使えないので/を使う）
                    case CLICK_SYMBOL.DIVIDE:
                        Show3DigitSeparator(LeftSideValue / nowValue);
                        break;
                }

                //今回利用した右辺の値を保存しておく
                RightSideValue = nowValue;

                //左辺は、計算結果の値を保存しておく（理由は前述）
                LeftSideValue = ConvertStringToDecimal(lblInputValue.Text);

                //状態を結果表示中に変更させる
                NowStatus = NOW_STATUS.SHOW_RESULT;
            }
            catch (DivideByZeroException)
            {
                //n÷0　は、プログラム上では処理できないため、例外エラーが発生する。
                //この場合は、それ以上処理ができないため、割ることができない旨をメッセージ表示して、表示をリセットさせる
                MessageBox.Show("0で割ることはできません", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
            catch (OverflowException)
            {
                //decimalが表現可能な範囲を超えた場合、例外エラーが発生する
                //この場合は、それ以上処理ができないため、最大桁数を超えた旨をメッセージ表示して、表示をリセットさせる
                MessageBox.Show("計算可能な最大桁数を超えました", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
        }
        #endregion

        #region 【CE】ボタンクリックイベント（最後に入力した数字のみクリア）
        /// <summary>
        /// 【CE】ボタンクリックイベント（最後に入力した数字のみクリア）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearEnd_Click(object sender, EventArgs e)
        {
            //現在入力中の数字をクリアする
            Show3DigitSeparator(ZERO);
        }
        #endregion

        #region 【C】ボタンクリックイベント（すべてクリア）
        /// <summary>
        /// 【C】ボタンクリックイベント（すべてクリア）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            //入力されたすべての値をクリアし、プログラム起動状態に戻す
            Reset();
        }
        #endregion

        #region 【←】（BackSpace）ボタンクリックイベント
        /// <summary>
        /// 【←】（BackSpace）ボタンクリックイベント
        /// 
        /// 右側から1文字ずつ削る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackSpace_Click(object sender, EventArgs e)
        {
            decimal nowValue = ConvertStringToDecimal(lblInputValue.Text);

            //現在の入力値が0の場合、何もしない(これ以上削ると文字が無くなってしまうため)
            if (nowValue == 0) return;

            //現在の入力値が1桁の場合は、これ以上削ると文字が無くなってしまうため、0に変える
            if (nowValue.ToString().Length == 1)
            {
                //0を表示する
                Show3DigitSeparator(ZERO);
            }
            else
            {
                //右側から1文字削る
                Show3DigitSeparator(nowValue.ToString().Substring(0, nowValue.ToString().Length - 1));
            }
        }
        #endregion

        #region 【±】ボタンクリックイベント
        /// <summary>
        /// 【±】ボタンクリックイベント
        /// 
        /// 符号を付ける／外す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSign_Click(object sender, EventArgs e)
        {
            //現在の入力値を×-1することで、符号を反転させる
            Show3DigitSeparator(ConvertStringToDecimal(lblInputValue.Text) * -1);
        }
        #endregion

        #region 【．】ボタンクリックイベント
        /// <summary>
        /// 【．】ボタンクリックイベント
        /// 
        /// 小数点を付ける
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecimalPoint_Click(object sender, EventArgs e)
        {
            //既に小数点が存在する場合は何もしない
            if (lblInputValue.Text.IndexOf(DECIMAL_POINT) > -1) return;

            //小数点を末尾に付ける
            Show3DigitSeparator(lblInputValue.Text + DECIMAL_POINT);
        }
        #endregion

        // ********** メニューイベント**********

        #region 設定→3桁区切りで表示メニュークリックイベント
        /// <summary>
        /// 設定→3桁区切りで表示メニュークリックイベント
        /// 
        /// 3桁区切りで表示メニューをクリックすると、1,234のように区切り文字が表示される。
        /// 再度クリックすると、1234のように区切り文字が消える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_3DigitSeparator_Click(object sender, EventArgs e)
        {
            bool showDecimalPoint;

            if (ToolStripMenuItem_3DigitSeparator.CheckState == CheckState.Checked)
            {
                ToolStripMenuItem_3DigitSeparator.CheckState = CheckState.Unchecked;
                showDecimalPoint = false;
            }
            else
            {
                ToolStripMenuItem_3DigitSeparator.CheckState = CheckState.Checked;
                showDecimalPoint = true;
            }

            //変更された設定を保存する
            Properties.Settings.Default.SHOW_3_DIGIT_SEPARATOR = showDecimalPoint;
            Properties.Settings.Default.Save();

            //小数点の設定が変更されたので、表示を更新する
            Show3DigitSeparator(lblInputValue.Text);
        }
        #endregion

        #region ヘルプメニュークリックイベント
        /// <summary>
        /// ヘルプメニュークリックイベント
        /// 
        /// 注意事項を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("この電卓は、月島ファクトリー様用に作成しました。\r\n" +
                "何卒宜しくお願いします。。\r\n" +
                "河原大徳", "ヘルプ",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        // ********** その他イベント **********

        #region URLリンクラベルクリックイベント
        /// <summary>
        /// URLリンクラベルクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //リンクをクリックしたことを登録する
            lnkURL.LinkVisited = true;
            //リンクラベルに記載されているURLをブラウザで開く（既定のブラウザが開く）
            System.Diagnostics.Process.Start(lnkURL.Text);
        }
        #endregion

        // ********** 画面表示 **********

        #region 3桁区切りに変換して表示(decimal用)
        /// <summary>
        /// 3桁区切りに変換して表示(decimal用)
        /// 
        /// このメソッドでは、受け取ったdecimal型の変数をstringに変換するだけで、
        /// 実際に3桁区切りにする処理は、同名のShow3DigitSeparator()メソッドに任せる。
        /// こうすることで、同じ変換処理を書かなくて済むことと、
        /// 呼び出し元でstringに変換しなくて済むので、ミスを防げる。
        /// </summary>
        /// <param name="value"></param>
        private void Show3DigitSeparator(decimal value)
        {
            Show3DigitSeparator(value.ToString());
        }
        #endregion

        #region 3桁区切りに変換して表示(string用)
        /// <summary>
        /// 3桁区切りに変換して表示(string用)
        /// 
        /// stringからdecimalに変換することで、decimal.ToString("#,0.############") をするだけで、
        /// 3桁区切りを実現している。
        /// </summary>
        /// <param name="value"></param>
        private void Show3DigitSeparator(string value)
        {
            decimal newValue = 0;
            bool firstDecimalPoint = false;

            //小数点が入力された直後か(例 10.)
            if (value.LastIndexOf(".") == value.Length - 1)
            {
                //小数点が入力された直後では、 10. のような値になっているが、
                //これをdecimalに変換すると、10になってしまい、小数点が失われてしまう。
                //そのため、まず末尾の.を除き、decimalに変換する。
                //このままでは.が除かれたままになるため、除いた旨をfirstDecimalPointフラグに記憶しておく

                //末尾の.を除いてdecimalに変換する
                newValue = ConvertStringToDecimal(value.Trim('.'));

                //小数点を除いた旨を記憶しておく
                firstDecimalPoint = true;
            }
            else
            {
                //小数点が未入力か、10.1のように小数点以下を入力中なので、
                //decimalに変換しても、小数点以下が失われないので、そのままstringからdecimalに変換
                newValue = ConvertStringToDecimal(value);
            }

            //3桁区切りで表示の切り替え
            if (Properties.Settings.Default.SHOW_3_DIGIT_SEPARATOR)
            {
                //3桁区切りが設定されている場合、#,0 とすることで、3桁を超えたら自動でカンマ区切り表示となる。
                value = newValue.ToString("#,0.############");
            }
            else
            {
                //3桁区切りが設定されていない場合
                value = newValue.ToString("0.############");
            }

            //小数点を入力し、まだそれ以降を入力していない場合(例 10. )、
            //decimalに変換すると小数点が消えてしまうため、ここで復元させている
            if (firstDecimalPoint)
            {
                value += ".";
            }

            //ここまでの処理で変換した結果を、ラベルに表示させる
            lblInputValue.Text = value;

            //計算結果を表示する際に、ラベルに収まるようにフォントサイズを調整する
            if (value.Length <= 17)
            {
                lblInputValue.Font = new System.Drawing.Font("Meiryo UI", 21.75F);
            }
            else if (value.Length <= 22)
            {
                lblInputValue.Font = new System.Drawing.Font("Meiryo UI", 16F);
            }
            else if (value.Length <= 32)
            {
                lblInputValue.Font = new System.Drawing.Font("Meiryo UI", 12F);
            }
            else
            {
                lblInputValue.Font = new System.Drawing.Font("Meiryo UI", 9F);
            }
        }
        #endregion

        // ********** 共通部品 **********

        #region stringをdecimalに変換
        /// <summary>
        /// stringをdecimalに変換
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private decimal ConvertStringToDecimal(string value)
        {
            //末尾に.がある場合、小数点未入力なので除去する
            value = value.Trim('.');

            //3桁区切りの,を除いて、stringからdecimalに変換する
            return decimal.Parse(value.Replace(",", ""));
        }
        #endregion

        #region 初期化（最初にプログラムを起動した状態に戻す）
        /// <summary>
        /// 初期化（最初にプログラムを起動した状態に戻す）
        /// </summary>
        private void Reset()
        {
            //画面の数字を0にする
            Show3DigitSeparator(ZERO);

            //状態を左辺入力中に変更
            NowStatus = NOW_STATUS.INPUT_LEFT_SIDE;

            //左辺と右辺の確定値を0にする
            LeftSideValue = 0;
            RightSideValue = 0;

            //＋‐×÷を未入力にする
            ClickSymbol = CLICK_SYMBOL.NONE;

            //3桁区切りが有効の設定になっている場合は、そのメニューの左側にチェックをつける
            if (Properties.Settings.Default.SHOW_3_DIGIT_SEPARATOR)
            {
                ToolStripMenuItem_3DigitSeparator.CheckState = CheckState.Checked;
            }

        }
        #endregion
    }
}