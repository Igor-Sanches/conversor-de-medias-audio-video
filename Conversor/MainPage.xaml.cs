#region Mídia Conversor
#region Using
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.Media.Transcoding;
using System.Threading; 
using Windows.System;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom; 
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage.FileProperties;
using Microsoft.Toolkit.Uwp.UI.Animations;
#endregion
//                                                                   *
  //                                                                 * *
///*******************************************************************   *
///  Projeto 7: Conversor de Mídia iniciado de 15/02/2018            *     *
///  Finalizado dia 23/02/2018 em 10 idiomas                         *       *
///  Desenvolvido por Igor De Jesus Dutra Sanches em Anajatuba - MA  *     *
///*******************************************************************   *
  //                                                                 * *
//                                                                   *
#region Conversor
namespace Conversor 
{
    sealed partial class MainPage : Page
    {
        //Compomentes para excução
        #region Componenetes
        ApplicationDataContainer _settings = ApplicationData.Current.LocalSettings;
        ResourceLoader loader = new ResourceLoader();
        StorageFile _file = null;
        StorageFile _fileC = null;
        MediaEncodingProfile _Profile;
        String _fileName = null;
        String _fileType = null;
        String _fileExtension = null;
       CancellationTokenSource _cts;
        MediaTranscoder _Transcoder = new MediaTranscoder();
        bool _mp4 = true;
        bool _avi = true;
        bool _m4a = true;
        bool _mp3 = true;
        bool _wav = true;
        bool _wma = true;
        bool _wmv = true;
        bool IsCelular = false;
        #endregion

        public MainPage()
        {
            //Inicialização
            #region Inicialização
            this.InitializeComponent();
            OpenMenu.Blur(30, 0, 0, 0).Start();
            _ProgressValue.Value = 0;
            ConfiguracoesUpdate();
            IsCelular2 = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
            Cts1 = new CancellationTokenSource();
            if (IsCelular2)
            {
                var statusbar = StatusBar.GetForCurrentView();
                statusbar.BackgroundColor = Colors.WhiteSmoke;
                statusbar.ForegroundColor = Colors.Black;
                statusbar.BackgroundOpacity = 100;
                tclsAtalho.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Comands.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SeparaC.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += MainPage_BackPressed;  
            }
            else {}
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
             if (!IsCelular2) Windows.UI.Xaml.Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            #endregion
          
        }
       
        //Butão voltar, somente para aparelhs mobiles
        #region Butão voltar... em aparelhos mobile

        void MainPage_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

          //  if (rootFrame.CanGoBack)
           
                if (GridSettings.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    GridSettings.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    GridPainel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
           
        }

        #endregion

        //Atualizar as configurações
        #region Atualizar Configurações

        void GoSupport()
        {
            if (mp4.IsChecked == false && mp3.IsChecked == false && m4a.IsChecked == false && avi.IsChecked == false && wma.IsChecked == false && wmv.IsChecked == false && wam.IsChecked == false) if (!Settings3.Values.ContainsKey("Support")) Settings3.Values.Add("Support", false); else Settings3.Values["Support"] = false; else if (!Settings3.Values.ContainsKey("Support")) Settings3.Values.Add("Support", true); else Settings3.Values["Support"] = true;
        }

        void ConfiguracoesUpdate()
        {
            //ToggleSwitch
            if (Settings3.Values.ContainsKey(tclsAtalho.Name)) tclsAtalho.IsOn = (bool)Settings3.Values[tclsAtalho.Name]; else tclsAtalho.IsOn = true;
            if (Settings3.Values.ContainsKey(AutoPlay.Name)) AutoPlay.IsOn = (bool)Settings3.Values[AutoPlay.Name]; else AutoPlay.IsOn = true;
            if (Settings3.Values.ContainsKey(Tile.Name)) Tile.IsOn = (bool)Settings3.Values[Tile.Name]; else Tile.IsOn = true;
            if (Settings3.Values.ContainsKey(cvtOption.Name)) cvtOption.IsOn = (bool)Settings3.Values[cvtOption.Name]; else cvtOption.IsOn = true;

            //CombBox revisão
         //   if (_settings.Values.ContainsKey(BoxEditors.Name)) BoxEditors.SelectedIndex = (int)_settings.Values[BoxEditors.Name]; else BoxEditors.SelectedIndex = 0;
            if (Settings3.Values.ContainsKey(CmbLanguage.Name)) CmbLanguage.SelectedIndex = (int)Settings3.Values[CmbLanguage.Name]; else CmbLanguage.SelectedIndex = 0;

            //Butão Radio revisão
            if (Settings3.Values.ContainsKey(AtivarNotificacao.Name)) AtivarNotificacao.IsChecked = (bool?)Settings3.Values[AtivarNotificacao.Name]; else AtivarNotificacao.IsChecked = true;
            if (Settings3.Values.ContainsKey(DesativarNotificacao.Name)) DesativarNotificacao.IsChecked = (bool?)Settings3.Values[DesativarNotificacao.Name]; else DesativarNotificacao.IsChecked = false;

            //CheckBox revisão
            if (Settings3.Values.ContainsKey(mp4.Name)) mp4.IsChecked = (bool?)Settings3.Values[mp4.Name]; else mp4.IsChecked = true;
            if (Settings3.Values.ContainsKey(mp3.Name)) mp3.IsChecked = (bool?)Settings3.Values[mp3.Name]; else mp3.IsChecked = true;
            if (Settings3.Values.ContainsKey(m4a.Name)) m4a.IsChecked = (bool?)Settings3.Values[m4a.Name]; else m4a.IsChecked = true;
            if (Settings3.Values.ContainsKey(avi.Name)) avi.IsChecked = (bool?)Settings3.Values[avi.Name]; else avi.IsChecked = true;
            if (Settings3.Values.ContainsKey(wam.Name)) wam.IsChecked = (bool?)Settings3.Values[wam.Name]; else wam.IsChecked = true;
            if (Settings3.Values.ContainsKey(wma.Name)) wma.IsChecked = (bool?)Settings3.Values[wma.Name]; else wma.IsChecked = true;
            if (Settings3.Values.ContainsKey(wmv.Name)) wmv.IsChecked = (bool?)Settings3.Values[wmv.Name]; else wmv.IsChecked = true;
            GoSupport();
            //*********** 
            if (_ProgressValue.Value > 0) BuscarFiles.IsEnabled = false;
            else
            {
                if (Settings3.Values.ContainsKey("Support")) BuscarFiles.IsEnabled = (bool)Settings3.Values["Support"]; else BuscarFiles.IsEnabled = true;
            }
            if (BuscarFiles.IsEnabled == false)
            {
                TargetFormat.SelectedIndex = 7; AudioOption.SelectedIndex = 3; VideoOption.SelectedIndex = 5;
                Mp4.Visibility = Windows.UI.Xaml.Visibility.Collapsed; MP3.Visibility = Windows.UI.Xaml.Visibility.Collapsed; M4A.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AVI.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WAV.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WMA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WMV.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                HD1080.Visibility = Windows.UI.Xaml.Visibility.Collapsed; HD720.Visibility = Windows.UI.Xaml.Visibility.Collapsed; QVGA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WVGA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VGA.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                BAIXA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; MEDIA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AUTA.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                FileNewFormat.Visibility = Windows.UI.Xaml.Visibility.Visible; FileNewFormat1.Visibility = Windows.UI.Xaml.Visibility.Visible; FileNewFormat2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Salvar.IsEnabled = false;
                MostrarOptions.IsEnabled = false;
            }
            else
            {
                Mp4.Visibility = Windows.UI.Xaml.Visibility.Visible; MP3.Visibility = Windows.UI.Xaml.Visibility.Visible; M4A.Visibility = Windows.UI.Xaml.Visibility.Visible; AVI.Visibility = Windows.UI.Xaml.Visibility.Visible; WAV.Visibility = Windows.UI.Xaml.Visibility.Visible; WMA.Visibility = Windows.UI.Xaml.Visibility.Visible; WMV.Visibility = Windows.UI.Xaml.Visibility.Visible;
                HD1080.Visibility = Windows.UI.Xaml.Visibility.Visible; HD720.Visibility = Windows.UI.Xaml.Visibility.Visible; QVGA.Visibility = Windows.UI.Xaml.Visibility.Visible; WVGA.Visibility = Windows.UI.Xaml.Visibility.Visible; VGA.Visibility = Windows.UI.Xaml.Visibility.Visible;
                BAIXA.Visibility = Windows.UI.Xaml.Visibility.Visible; MEDIA.Visibility = Windows.UI.Xaml.Visibility.Visible; AUTA.Visibility = Windows.UI.Xaml.Visibility.Visible;
                FileNewFormat.Visibility = Windows.UI.Xaml.Visibility.Collapsed; FileNewFormat1.Visibility = Windows.UI.Xaml.Visibility.Collapsed; FileNewFormat2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Salvar.IsEnabled = true;
                MostrarOptions.IsEnabled = true;
                if (Settings3.Values.ContainsKey(TargetFormat.Name)) TargetFormat.SelectedIndex = (int)Settings3.Values[TargetFormat.Name]; else TargetFormat.SelectedIndex = 1;
                if (Settings3.Values.ContainsKey(AudioOption.Name)) AudioOption.SelectedIndex = (int)Settings3.Values[AudioOption.Name]; else AudioOption.SelectedIndex = 1;
                if (Settings3.Values.ContainsKey(VideoOption.Name)) VideoOption.SelectedIndex = (int)Settings3.Values[VideoOption.Name]; else VideoOption.SelectedIndex = 1;
            }
            
            if (AutoPlay.IsOn == true) MediaView1.AutoPlay = true; else MediaView1.AutoPlay = false;

            #region MidiasVisiveisIndex
            if (mp4.IsChecked == false) { Mp4.Visibility = Windows.UI.Xaml.Visibility.Collapsed; if (TargetFormat.SelectedIndex == 0) { if (avi.IsChecked == true) { TargetFormat.SelectedIndex = 1; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                    else { if (m4a.IsChecked == true) { TargetFormat.SelectedIndex = 2; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                        else { if (mp3.IsChecked == true) { TargetFormat.SelectedIndex = 3; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                            else { if (wam.IsChecked == true) { TargetFormat.SelectedIndex = 4; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                else { if (wma.IsChecked == true) { TargetFormat.SelectedIndex = 5; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); ; }
                                    else { if (wmv.IsChecked == true) { TargetFormat.SelectedIndex = 6; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                        else { TargetFormat.SelectedIndex = 7; } } } } } } } } else Mp4.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (avi.IsChecked == false) { AVI.Visibility = Windows.UI.Xaml.Visibility.Collapsed; if (TargetFormat.SelectedIndex == 1) { if (mp4.IsChecked == true) { TargetFormat.SelectedIndex = 0; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                    else { if (m4a.IsChecked == true) { TargetFormat.SelectedIndex = 2; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                        else { if (mp3.IsChecked == true) { TargetFormat.SelectedIndex = 3; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                            else { if (wam.IsChecked == true) { TargetFormat.SelectedIndex = 4; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                else { if (wma.IsChecked == true) { TargetFormat.SelectedIndex = 5; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                    else { if (wmv.IsChecked == true) { TargetFormat.SelectedIndex = 6; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                        else { TargetFormat.SelectedIndex = 7; } } } } } } } } else AVI.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (m4a.IsChecked == false) { M4A.Visibility = Windows.UI.Xaml.Visibility.Collapsed; if (TargetFormat.SelectedIndex == 2) { if (mp4.IsChecked == true) { TargetFormat.SelectedIndex = 0; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                    else { if (avi.IsChecked == true) { TargetFormat.SelectedIndex = 1; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                        else { if (mp3.IsChecked == true) { TargetFormat.SelectedIndex = 3; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                            else { if (wam.IsChecked == true) { TargetFormat.SelectedIndex = 4; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                else { if (wma.IsChecked == true) { TargetFormat.SelectedIndex = 5; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                    else { if (wmv.IsChecked == true) { TargetFormat.SelectedIndex = 6; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                        else { TargetFormat.SelectedIndex = 7; } } } } } } } } else M4A.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (mp3.IsChecked == false) { MP3.Visibility = Windows.UI.Xaml.Visibility.Collapsed; if (TargetFormat.SelectedIndex == 3) { if (mp4.IsChecked == true) { TargetFormat.SelectedIndex = 0; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                    else { if (avi.IsChecked == true) { TargetFormat.SelectedIndex = 1; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                        else { if (m4a.IsChecked == true) { TargetFormat.SelectedIndex = 2; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                            else { if (wam.IsChecked == true) { TargetFormat.SelectedIndex = 4; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                else { if (wma.IsChecked == true) { TargetFormat.SelectedIndex = 5; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                    else { if (wmv.IsChecked == true) { TargetFormat.SelectedIndex = 6; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                        else { TargetFormat.SelectedIndex = 7; } } } } } } } } else MP3.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (wam.IsChecked == false) { WAV.Visibility = Windows.UI.Xaml.Visibility.Collapsed; if (TargetFormat.SelectedIndex == 4) { if (mp4.IsChecked == true) { TargetFormat.SelectedIndex = 0; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                    else { if (avi.IsChecked == true) { TargetFormat.SelectedIndex = 1; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                        else { if (m4a.IsChecked == true) { TargetFormat.SelectedIndex = 2; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                            else { if (mp3.IsChecked == true) { TargetFormat.SelectedIndex = 3; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                else { if (wma.IsChecked == true) { TargetFormat.SelectedIndex = 5; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                    else { if (wmv.IsChecked == true) { TargetFormat.SelectedIndex = 6; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                        else { TargetFormat.SelectedIndex = 7; } } } } } } } } else WAV.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (wma.IsChecked == false) { WMA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; if (TargetFormat.SelectedIndex == 5) { if (mp4.IsChecked == true) { TargetFormat.SelectedIndex = 0; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                    else { if (avi.IsChecked == true) { TargetFormat.SelectedIndex = 1; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                        else { if (m4a.IsChecked == true) { TargetFormat.SelectedIndex = 2; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                            else { if (mp3.IsChecked == true) { TargetFormat.SelectedIndex = 3; MostrarAudioEditor(); VideoAudioSelectedIndex(); }
                                else { if (wam.IsChecked == true) { TargetFormat.SelectedIndex = 4; MostrarAudioEditor(); VideoAudioSelectedIndex(); }
                                    else { if (wmv.IsChecked == true) { TargetFormat.SelectedIndex = 6; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                        else { TargetFormat.SelectedIndex = 7; } } } } } } } } else WMA.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (wmv.IsChecked == false) { WMV.Visibility = Windows.UI.Xaml.Visibility.Collapsed; if (TargetFormat.SelectedIndex == 6) { if (mp4.IsChecked == true) { TargetFormat.SelectedIndex = 0; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                    else { if (avi.IsChecked == true) { TargetFormat.SelectedIndex = 1; MostrarVideoEditor(); AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible; VideoOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                        else { if (m4a.IsChecked == true) { TargetFormat.SelectedIndex = 2; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                            else { if (mp3.IsChecked == true) { TargetFormat.SelectedIndex = 3; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                else { if (wam.IsChecked == true) { TargetFormat.SelectedIndex = 4; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                    else { if (wma.IsChecked == true) { TargetFormat.SelectedIndex = 5; MostrarAudioEditor(); VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible; AudioOption.SelectedIndex = 1; VideoAudioSelectedIndex(); }
                                        else { TargetFormat.SelectedIndex = 7; } } } } } } } } else WMV.Visibility = Windows.UI.Xaml.Visibility.Visible;
#endregion
        }

      
        #endregion

        //Area dos comandos
        #region Comandos

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            var ctrl = Windows.UI.Xaml.Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            if (tclsAtalho.IsOn)
            {
                if (ctrl.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.F1:
                            if (GridSettings.Visibility == Windows.UI.Xaml.Visibility.Collapsed &&
                                GridPainel.Visibility == Windows.UI.Xaml.Visibility.Visible) BuscaNovoArqquivo();
                            break;
                        case VirtualKey.F2:
                            if (GridSettings.Visibility == Windows.UI.Xaml.Visibility.Collapsed &&
                                GridPainel.Visibility == Windows.UI.Xaml.Visibility.Visible) SalvarEConverter();
                            break;
                        case VirtualKey.F3:
                            if (File1 != null || FileC1 != null && GridPainel.Visibility == Windows.UI.Xaml.Visibility.Visible) MostraEOcultar();
                            break;
                        case VirtualKey.F4:
                            if (AtivarNotificacao.IsChecked == false)
                            { AtivarNotificacao.IsChecked = true; DesativarNotificacao.IsChecked = false; if (!Settings3.Values.ContainsKey(AtivarNotificacao.Name)) Settings3.Values.Add(AtivarNotificacao.Name, true); else Settings3.Values[AtivarNotificacao.Name] = true;
                                if (!Settings3.Values.ContainsKey(DesativarNotificacao.Name)) Settings3.Values.Add(DesativarNotificacao.Name, false); else Settings3.Values[DesativarNotificacao.Name] = false;
                            }
                            else { AtivarNotificacao.IsChecked = false; DesativarNotificacao.IsChecked = true; if (!Settings3.Values.ContainsKey(AtivarNotificacao.Name)) Settings3.Values.Add(AtivarNotificacao.Name, false); else Settings3.Values[AtivarNotificacao.Name] = false;
                                if (!Settings3.Values.ContainsKey(DesativarNotificacao.Name)) Settings3.Values.Add(DesativarNotificacao.Name, true); else Settings3.Values[DesativarNotificacao.Name] = true;
                            }
                            break;
                        //  case VirtualKey.F5:
                        //      DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; ModernEditor.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        //     BoxEditors.SelectedIndex = 1; if (!_settings.Values.ContainsKey(BoxEditors.Name)) _settings.Values.Add(BoxEditors.Name, 1); else _settings.Values[BoxEditors.Name] = 1;
                        //     break;
                        case VirtualKey.F5:
                            if (!Tile.IsOn) Tile.IsOn = true; if (!Settings3.Values.ContainsKey(Tile.Name)) Settings3.Values.Add(Tile.Name, true); else Settings3.Values[Tile.Name] = true;
                            break;
                        case VirtualKey.F6:
                            DataTransferManager.ShowShareUI();
                            break;
                        default: break;
                    }
                }
            }
        }

        #endregion

        //Configurações
        #region Configurações

        private void Toggles_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch toggle = ((ToggleSwitch)sender);
            bool isOn = toggle.IsOn; var Nome = toggle.Name;
            if (!Settings3.Values.ContainsKey(Nome)) Settings3.Values.Add(Nome, isOn); else Settings3.Values[Nome] = isOn;
        }

        #region Idioma

        private async void LanguageSelected_Tapped(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var content = Loader1.GetString("textContent");
            var label0 = Loader1.GetString("textLabel0");
            var label1 = Loader1.GetString("textLabel");
            var messageDialog = new MessageDialog(content);
            messageDialog.Commands.Add(new UICommand(label0, (command) =>
            {
                switch (CmbLanguage.SelectedIndex)
                {
                    case 0: //Padrao do sistema
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", ""); else Settings3.Values["Language"] = "";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 0); else Settings3.Values[CmbLanguage.Name] = 0;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 1: //Arabe
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "ar"); else Settings3.Values["Language"] = "ar";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 1); else Settings3.Values[CmbLanguage.Name] = 1;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 2: //Chines (Simplificado)
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "zh-Hans"); else Settings3.Values["Language"] = "zh-Hans";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 2); else Settings3.Values[CmbLanguage.Name] = 2;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 3: //Chines (Tradicional)
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "zh-Hant"); else Settings3.Values["Language"] = "zh-Hant";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 3); else Settings3.Values[CmbLanguage.Name] = 3;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 4: //Esplanhol
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "es"); else Settings3.Values["Language"] = "es";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 4); else Settings3.Values[CmbLanguage.Name] = 4;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 5: //Frances 
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "fr-FR"); else Settings3.Values["Language"] = "fr-FR";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 5); else Settings3.Values[CmbLanguage.Name] = 5;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 6: //Ingles
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "en-US"); else Settings3.Values["Language"] = "en-US";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 6); else Settings3.Values[CmbLanguage.Name] = 6;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 7: //Italiano
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "it-IT"); else Settings3.Values["Language"] = "it-IT";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 7); else Settings3.Values[CmbLanguage.Name] = 7;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 8: //Japones
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "ja"); else Settings3.Values["Language"] = "ja";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 8); else Settings3.Values[CmbLanguage.Name] = 8;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 9: //Portugues
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "pt-BR"); else Settings3.Values["Language"] = "pt-BR";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 9); else Settings3.Values[CmbLanguage.Name] = 9;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 10: //Russo
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "ru-RU"); else Settings3.Values["Language"] = "ru-RU";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 10); else Settings3.Values[CmbLanguage.Name] = 10;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                    case 11: //Viatnamita
                        if (!Settings3.Values.ContainsKey("Language")) Settings3.Values.Add("Language", "vi"); else Settings3.Values["Language"] = "vi";
                        if (!Settings3.Values.ContainsKey(CmbLanguage.Name)) Settings3.Values.Add(CmbLanguage.Name, 11); else Settings3.Values[CmbLanguage.Name] = 11;
                        if (Settings3.Values.ContainsKey("Language")) ApplicationLanguages.PrimaryLanguageOverride = (string)Settings3.Values[("Language")];
                        { App.Current.Exit(); }
                        break;
                }
            }));
            messageDialog.Commands.Add(new UICommand(label1, (c) =>
            {
                if (Settings3.Values.ContainsKey(CmbLanguage.Name)) CmbLanguage.SelectedIndex = (int)Settings3.Values[CmbLanguage.Name]; else CmbLanguage.SelectedIndex = 0;
            }, 1));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;
            await messageDialog.ShowAsync();
        }
        #endregion

        private async void Tile_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Tile.IsOn)
            {
                StorageFolder lOCAL = ApplicationData.Current.LocalFolder;
                var pasta = await lOCAL.CreateFolderAsync("Tarefa", CreationCollisionOption.OpenIfExists);
                var arquivo = await pasta.CreateFileAsync("Tarefa", CreationCollisionOption.OpenIfExists);
                var folderws = await lOCAL.GetFolderAsync("Tarefa");
                var file2s = await folderws.GetFileAsync("Tarefa");
                String text = await FileIO.ReadTextAsync(file2s);
                ChamaTile310x150(text);
            }
            else { TileUpdateManager.CreateTileUpdaterForApplication().Clear(); }
            bool isOn = Tile.IsOn; var Nome = Tile.Name;
            if (!Settings3.Values.ContainsKey(Nome)) Settings3.Values.Add(Nome, isOn); else Settings3.Values[Nome] = isOn;
        }

        private void BoxEditors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //       int index = BoxEditors.SelectedIndex; var Nome = BoxEditors.Name;
            //      if (!_settings.Values.ContainsKey(Nome)) _settings.Values.Add(Nome, index); else _settings.Values[Nome] = index;
        }

        private void AtivarNotificacao_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bool? isChecked1 = AtivarNotificacao.IsChecked; var Nome1 = AtivarNotificacao.Name;
            if (!Settings3.Values.ContainsKey(Nome1)) Settings3.Values.Add(Nome1, isChecked1); else Settings3.Values[Nome1] = isChecked1;
            bool? isChecked = DesativarNotificacao.IsChecked; var Nome = DesativarNotificacao.Name;
            if (!Settings3.Values.ContainsKey(Nome)) Settings3.Values.Add(Nome, isChecked); else Settings3.Values[Nome] = isChecked;
        }

        private void DesativarNotificacao_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bool? isChecked = DesativarNotificacao.IsChecked; var Nome = DesativarNotificacao.Name;
            if (!Settings3.Values.ContainsKey(Nome)) Settings3.Values.Add(Nome, isChecked); else Settings3.Values[Nome] = isChecked;
            bool? isChecked1 = AtivarNotificacao.IsChecked; var Nome1 = AtivarNotificacao.Name;
            if (!Settings3.Values.ContainsKey(Nome1)) Settings3.Values.Add(Nome1, isChecked1); else Settings3.Values[Nome1] = isChecked1;
        }

        private void CheckBox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = ((CheckBox)sender);
            bool? isChecked = checkBox.IsChecked; var Nome = checkBox.Name;
            if (!Settings3.Values.ContainsKey(Nome)) Settings3.Values.Add(Nome, isChecked); else Settings3.Values[Nome] = isChecked;
        }

        #endregion

        //Buscar, Adicionar e Converter
        #region Buscar e Salvar conversão

        private async void SalvarEConverter()
        {
            if (_NomeDoArquivo.Text == "") FileName1 = _NomeDoArquivo.PlaceholderText; else FileName1 = _NomeDoArquivo.Text;
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            picker.SuggestedFileName = FileName1;
            picker.FileTypeChoices.Add(FileType1, new System.Collections.Generic.List<string>() { FileExtension1 });
            FileC1 = await picker.PickSaveFileAsync();
            if (FileC1 != null)
            {
                 VideoAudioSelectedIndex();
                Conversao();
                va.Width = Salvar.ActualWidth;
            }
        }

        private async void BuscaNovoArqquivo()
        {
            FileOpenPicker _arquivo = new FileOpenPicker();
            _arquivo.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            _arquivo.ViewMode = PickerViewMode.List;
            if (mp4.IsChecked == true) _arquivo.FileTypeFilter.Add(".mp4");
            if (mp3.IsChecked == true) _arquivo.FileTypeFilter.Add(".mp3");
            if (m4a.IsChecked == true) _arquivo.FileTypeFilter.Add(".m4a");
            if (avi.IsChecked == true) _arquivo.FileTypeFilter.Add(".avi");
            if (wam.IsChecked == true) _arquivo.FileTypeFilter.Add(".wav");
            if (wma.IsChecked == true) _arquivo.FileTypeFilter.Add(".wma");
            if (wmv.IsChecked == true) _arquivo.FileTypeFilter.Add(".wmv");
            File1 = await _arquivo.PickSingleFileAsync();
            if (File1 != null)
            {
                IRandomAccessStream stream = await File1.OpenAsync(FileAccessMode.Read);

                var F8 = Loader1.GetString("autos");
                if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                {
                     if (File1.FileType == ".mp3") TargetFormat.SelectedIndex = 3; else if (File1.FileType == ".m4a") TargetFormat.SelectedIndex = 2; else if (File1.FileType == ".wma") TargetFormat.SelectedIndex = 4; else if (File1.FileType == ".wav") TargetFormat.SelectedIndex = 5;
                    MediaView1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    AudioElementOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    var _music = await File1.Properties.GetMusicPropertiesAsync();
                    InicioDoCorte.Maximum = _music.Duration.TotalMinutes;
                    FinDocorte.Maximum = _music.Duration.TotalMinutes;
                    MusicProperties musicProperties = await File1.Properties.GetMusicPropertiesAsync();
                    mscTitulo.PlaceholderText = musicProperties.Title;
                    mscAlbum.PlaceholderText = musicProperties.Album;
                    mscArtista.PlaceholderText = musicProperties.Artist;
                    mscArtistaAlbum.PlaceholderText = musicProperties.AlbumArtist;
                    VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    TagEditor.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    VideoEdition2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    AudioEdition2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                {
                   if (File1.FileType == ".mp4") TargetFormat.SelectedIndex = 0; else if (File1.FileType == ".avi") TargetFormat.SelectedIndex = 1; else if (File1.FileType == ".wmv") TargetFormat.SelectedIndex = 6;
                    AudioElementOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    MediaView1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    var _video = await File1.Properties.GetVideoPropertiesAsync();
                    InicioDoCorte.Maximum = _video.Duration.TotalMinutes;
                    FinDocorte.Maximum = _video.Duration.TotalMinutes;
                    AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    TagEditor.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    VideoEdition2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    AudioEdition2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }

                VideoAudioSelectedIndex();
                AudioOption.SelectedIndex = 1;
                VideoOption.SelectedIndex = 1;
                Index();
                AtualizarOMesmo();
                var originalFormat = Loader1.GetString("originalFormat");
                Criacao.Text = $"{originalFormat} {File1.FileType}";
                CampoBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Arquivocriacao.Date = File1.DateCreated;
                _NomeDoArquivo.PlaceholderText = File1.DisplayName;
                _NomeDoArquivo.Visibility = Windows.UI.Xaml.Visibility.Visible;
                GridSettings.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                GridPainel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                MostrarOptions.Visibility = Windows.UI.Xaml.Visibility.Visible;
                NullOuNot();
                RootGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                MediaView1.SetSource(stream, File1.ContentType);
            }
        }

        private void BuscarFiles_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BuscaNovoArqquivo();
        }

        private void Salvar_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SalvarEConverter();
        }

        private void Cancelar_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            File1 = null; FileC1 = null;
            MediaView1.Stop();
            RootGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioElementOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            MediaView1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            NullOuNot();
        }

        private void NullOuNot()
        {
            if (File1 == null && FileC1 == null)
            {
                SwepararBuscarCancelar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //   Cancelar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Separar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Salvar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                SwepararBuscarCancelar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //   Cancelar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Separar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Salvar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Salvar.IsEnabled = true;
            }
        }

        #endregion

        //Butões visiveis e invisiveis
        #region Butões visiveis e invisiveis

        void visiveis()
        {
            Salvar.IsEnabled = true;
            BuscarFiles.IsEnabled = true;
            //  Cancelar.IsEnabled = true;
            Mp4.Visibility = Windows.UI.Xaml.Visibility.Visible; MP3.Visibility = Windows.UI.Xaml.Visibility.Visible; M4A.Visibility = Windows.UI.Xaml.Visibility.Visible; AVI.Visibility = Windows.UI.Xaml.Visibility.Visible; WAV.Visibility = Windows.UI.Xaml.Visibility.Visible; WMA.Visibility = Windows.UI.Xaml.Visibility.Visible; WMV.Visibility = Windows.UI.Xaml.Visibility.Visible;
            HD1080.Visibility = Windows.UI.Xaml.Visibility.Visible; HD720.Visibility = Windows.UI.Xaml.Visibility.Visible; QVGA.Visibility = Windows.UI.Xaml.Visibility.Visible; WVGA.Visibility = Windows.UI.Xaml.Visibility.Visible; VGA.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BAIXA.Visibility = Windows.UI.Xaml.Visibility.Visible; MEDIA.Visibility = Windows.UI.Xaml.Visibility.Visible; AUTA.Visibility = Windows.UI.Xaml.Visibility.Visible;
            FileNewFormat.Visibility = Windows.UI.Xaml.Visibility.Collapsed; FileNewFormat1.Visibility = Windows.UI.Xaml.Visibility.Collapsed; FileNewFormat2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            MostrarOptions.IsEnabled = true;
            if (Settings3.Values.ContainsKey(TargetFormat.Name)) TargetFormat.SelectedIndex = (int)Settings3.Values[TargetFormat.Name]; else TargetFormat.SelectedIndex = 1;
            if (Settings3.Values.ContainsKey(AudioOption.Name)) AudioOption.SelectedIndex = (int)Settings3.Values[AudioOption.Name]; else AudioOption.SelectedIndex = 1;
            if (Settings3.Values.ContainsKey(VideoOption.Name)) VideoOption.SelectedIndex = (int)Settings3.Values[VideoOption.Name]; else VideoOption.SelectedIndex = 1;
        }

        void invisiveis()
        {
            Salvar.IsEnabled = false;
            BuscarFiles.IsEnabled = false;
            //   Cancelar.IsEnabled = false;
            TargetFormat.SelectedIndex = 7; AudioOption.SelectedIndex = 3; VideoOption.SelectedIndex = 5;
            Mp4.Visibility = Windows.UI.Xaml.Visibility.Collapsed; MP3.Visibility = Windows.UI.Xaml.Visibility.Collapsed; M4A.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AVI.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WAV.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WMA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WMV.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            HD1080.Visibility = Windows.UI.Xaml.Visibility.Collapsed; HD720.Visibility = Windows.UI.Xaml.Visibility.Collapsed; QVGA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; WVGA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; VGA.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            BAIXA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; MEDIA.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AUTA.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            FileNewFormat.Visibility = Windows.UI.Xaml.Visibility.Visible; FileNewFormat1.Visibility = Windows.UI.Xaml.Visibility.Visible; FileNewFormat2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            MostrarOptions.IsEnabled = false;
        }

        #endregion

        //Conversão completa, em progresso, falhada e cancelada
        #region Conversão

        private void TargetFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             switch (TargetFormat.SelectedIndex)
            {
                case 0:
                    FileExtension1 = ".mp4";
                    FileType1 = "MP4";
                    Mp42 = true;
                    Avi1 = false;
                    M4a1 = false;
                    Mp31 = false;
                    Wav1 = false;
                    Wma1 = false;
                    Wmv1 = false;
                    break;
                case 1:
                    FileExtension1 = ".avi";
                    FileType1 = "AVI";
                    Mp42 = false;
                    Avi1 = true;
                    M4a1 = false;
                    Mp31 = false;
                    Wav1 = false;
                    Wma1 = false;
                    Wmv1 = false;
                    break;
                case 2:
                    FileExtension1 = ".m4a";
                    FileType1 = "M4A";
                    Mp42 = false;
                    Avi1 = false;
                    M4a1 = true;
                    Mp31 = false;
                    Wav1 = false;
                    Wma1 = false;
                    Wmv1 = false;
                    break;
                case 3:
                    FileExtension1 = ".mp3";
                    FileType1 = "MP3";
                    Mp42 = false;
                    Avi1 = false;
                    M4a1 = false;
                    Mp31 = true;
                    Wav1 = false;
                    Wma1 = false;
                    Wmv1 = false;
                    break;
                case 4:
                    FileExtension1 = ".wav";
                    FileType1 = "WAV";
                    Mp42 = false;
                    Avi1 = false;
                    M4a1 = false;
                    Mp31 = false;
                    Wav1 = true;
                    Wma1 = false;
                    Wmv1 = false;
                    break;
                case 5:
                    FileExtension1 = ".wma";
                    FileType1 = "WMA";
                    Mp42 = false;
                    Avi1 = false;
                    M4a1 = false;
                    Mp31 = false;
                    Wav1 = false;
                    Wma1 = true;
                    Wmv1 = false;
                    break;
                case 6:
                    FileExtension1 = ".wmv";
                    FileType1 = "WMV";
                    Mp42 = false;
                    Avi1 = false;
                    M4a1 = false;
                    Mp31 = false;
                    Wav1 = false;
                    Wma1 = false;
                    Wmv1 = true;
                    break;
            }
        }

        public void Dispose() => Cts1.Dispose();

        private async void Conversao()
        {
            GetCustomProfile();
            try
            {
                if (File1 != null && Profile1 != null && FileC1 != null)
                {
                    var preparedTranscodeResult = await Transcoder1.PrepareFileTranscodeAsync(File1, FileC1, Profile1);

                    if (cvtOption.IsOn)
                    {
                        Transcoder1.VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.MrfCrf444;
                    }
                    else
                    {
                        Transcoder1.VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.Default;
                    }

                    if (preparedTranscodeResult.CanTranscode)
                    {
                        invisiveis();  
                        var progress = new Progress<double>(ProgressoConversao);
                        await preparedTranscodeResult.TranscodeAsync().AsTask(Cts1.Token, progress);
                        ConversaoCompletada();
                    }
                    else
                    {
                       
                        FalhaNaConversao(preparedTranscodeResult.FailureReason);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                //  va.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                //   va.Visibility = Visibility.Visible;
            }
        }

        private void GetCustomProfile()
        {
            Profile1 = null;

            if(Mp42 == true)
            {
                 
                switch (VideoOption.SelectedIndex)
                {
                    case 0:
                        Profile1 = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD1080p);
                        break;
                    case 1:
                        Profile1 = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD720p);
                        break;
                    case 2:
                        Profile1 = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Wvga);
                        break;
                    case 3:
                        Profile1 = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Vga);
                        break;
                    case 4:
                        Profile1 = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Qvga);
                        break;
                }
            }
            else if(Avi1 == true)
            {
                
                switch (VideoOption.SelectedIndex)
                {
                    case 0:
                        Profile1 = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.HD1080p);
                        break;
                    case 1:
                        Profile1 = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.HD720p);
                        break;
                    case 2:
                        Profile1 = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Wvga);
                        break;
                    case 3:
                        Profile1 = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Vga);
                        break;
                    case 4:
                        Profile1 = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Qvga);
                        break;
                }
            }
            else if(M4a1 == true)
            {
               
                switch (AudioOption.SelectedIndex)
                {
                    case 0:
                        Profile1 = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Low);
                        break;
                    case 1:
                        Profile1 = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Medium);
                        break;
                    case 2:
                        Profile1 = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.High);
                        break;
                }
            }
            else if(Mp31 == true)
            {
                switch (AudioOption.SelectedIndex)
                {
                    case 0:
                        Profile1 = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Low);
                        break;
                    case 1:
                        Profile1 = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Medium);
                        break;
                    case 2:
                        Profile1 = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High);
                        break;
                }
            }
            else if(Wav1 == true)
            {
                
                switch (AudioOption.SelectedIndex)
                {
                    case 0:
                        Profile1 = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);
                        break;
                    case 1:
                        Profile1 = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);
                        break;
                    case 2:
                        Profile1 = MediaEncodingProfile.CreateWav(AudioEncodingQuality.High);
                        break;
                }
            }
            else if (Wma1)
            {
                
                switch (AudioOption.SelectedIndex)
                {
                    case 0:
                        Profile1 = MediaEncodingProfile.CreateWma(AudioEncodingQuality.Low);
                        break;
                    case 1:
                        Profile1 = MediaEncodingProfile.CreateWma(AudioEncodingQuality.Medium);
                        break;
                    case 2:
                        Profile1 = MediaEncodingProfile.CreateWma(AudioEncodingQuality.High);
                        break;
                }
            }
            else if (Wmv1 == true)
            {
                
                switch (VideoOption.SelectedIndex)
                {

                    case 0:
                        Profile1 = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.HD1080p);
                        break;
                    case 1:
                        Profile1 = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.HD720p);
                        break;
                    case 2:
                        Profile1 = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Wvga);
                        break;
                    case 3:
                        Profile1 = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Vga);
                        break;
                    case 4:
                        Profile1 = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Qvga);
                        break;
                }
            }

             try
                {
                    if (VideoW.Text == "") Profile1.Video.Width = UInt32.Parse(VideoW.PlaceholderText); else Profile1.Video.Width = UInt32.Parse(VideoW.Text);
                    if (VideoH.Text == "") Profile1.Video.Height = UInt32.Parse(VideoH.PlaceholderText); else Profile1.Video.Height = UInt32.Parse(VideoH.Text);
                    if (VideoBR.Text == "") Profile1.Video.Bitrate = UInt32.Parse(VideoBR.PlaceholderText); else Profile1.Video.Bitrate = UInt32.Parse(VideoBR.Text);
                    if (VideoFR.Text == "") Profile1.Video.FrameRate.Numerator = UInt32.Parse(VideoFR.PlaceholderText); else Profile1.Video.FrameRate.Numerator = UInt32.Parse(VideoFR.Text);
                    Profile1.Video.FrameRate.Denominator = 1;
                    if (AudioBPS.Text == "") Profile1.Audio.BitsPerSample = UInt32.Parse(AudioBPS.PlaceholderText); else Profile1.Audio.BitsPerSample = UInt32.Parse(AudioBPS.Text);
                    if (AudioCC.Text == "") Profile1.Audio.ChannelCount = UInt32.Parse(AudioCC.PlaceholderText); else Profile1.Audio.ChannelCount = UInt32.Parse(AudioCC.Text);
                    if (AudioBR.Text == "") Profile1.Audio.Bitrate = UInt32.Parse(AudioBR.PlaceholderText); else Profile1.Audio.Bitrate = UInt32.Parse(AudioBR.Text);
                    if (AudioSR.Text == "") Profile1.Audio.SampleRate = UInt32.Parse(AudioSR.PlaceholderText); else Profile1.Audio.SampleRate = UInt32.Parse(AudioSR.Text);
                if (InicioDoCorte.Value > 0)
                {
                    var ini = TimeSpan.FromSeconds(InicioDoCorte.Value);
                    var fini = TimeSpan.FromSeconds(FinDocorte.Value);
                    Transcoder1.TrimStartTime = ini;
                    Transcoder1.TrimStopTime = fini;
                }
                if (ResolucaoWidth.Text == "") Profile1.Video.Width = UInt32.Parse(ResolucaoWidth.PlaceholderText); else Profile1.Video.Width = UInt32.Parse(ResolucaoWidth.Text);
                if (ResolucaoHeight.Text == "") Profile1.Video.Height = UInt32.Parse(ResolucaoHeight.PlaceholderText); else Profile1.Video.Height = UInt32.Parse(ResolucaoHeight.Text);
                Profile1.Video.FrameRate.Numerator = UInt32.Parse(VideoFR2.Value.ToString());
                Profile1.Video.FrameRate.Denominator = 1;
                Profile1.Audio.BitsPerSample = UInt32.Parse(AudioBPS2.Value.ToString());
            }
            catch (Exception exception)
                {
                    // TranscodeError(exception.Message);
                  //  _Profile = null;
                }
            
        }
         
        private async void FalhaNaConversao(TranscodeFailureReason failureReason)
        {
            try
            {
                if (FileC1 != null)
                {
                    await FileC1.DeleteAsync();
                }
                visiveis();
                var falha = Loader1.GetString("falha");
                var Motivofalha = Loader1.GetString("falha2");
                if (AtivarNotificacao.IsChecked == true && DesativarNotificacao.IsChecked == false)
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(falha));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode(Motivofalha));
                    XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/StoreLogo.png");
                    ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));
                }
                else if (DesativarNotificacao.IsChecked == true && AtivarNotificacao.IsChecked == false)
                {
                    this.RootContextContainer.Children.Clear();
                    this.RootContextTitle.Text = falha;
                    this.RootContextContainer.Children.Add(new TextBlock() { Text = Motivofalha, Style = this.Resources["RootContextTextStyle"] as Windows.UI.Xaml.Style });
                    this.RootContentContext.ShowAt(this);
                }
            }
            catch (Exception exception)
            {
                //  TranscodeError(exception.Message);
                var falha = Loader1.GetString("falha");
                if (AtivarNotificacao.IsChecked == true && DesativarNotificacao.IsChecked == false)
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(falha));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode(exception.Message));
                    XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/StoreLogo.png");
                    ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));
                }
                else if (DesativarNotificacao.IsChecked == true && AtivarNotificacao.IsChecked == false)
                {
                    this.RootContextContainer.Children.Clear();
                    this.RootContextTitle.Text = falha;
                    this.RootContextContainer.Children.Add(new TextBlock() { Text = exception.Message, Style = this.Resources["RootContextTextStyle"] as Windows.UI.Xaml.Style });
                    this.RootContentContext.ShowAt(this);
                }
            }

            var Conversao = Loader1.GetString("Conversao");
            var convertidoDe = Loader1.GetString("convertidoDe");
            var convertidoPara = Loader1.GetString("convertidoPara");
            var convertidoFalha = Loader1.GetString("convertidoFalha");
            var convertidoAs = Loader1.GetString("convertidoAs");
            StorageFolder lOCAL = ApplicationData.Current.LocalFolder;
            var pasta = await lOCAL.CreateFolderAsync("Tarefa", CreationCollisionOption.OpenIfExists);
            var arquivo = await pasta.CreateFileAsync("Tarefa", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(arquivo, $"{Conversao}\n{convertidoDe} {File1.FileType} {convertidoPara} {FileC1.FileType}\n{convertidoFalha}\n{convertidoAs} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
            var folderws = await lOCAL.GetFolderAsync("Tarefa");
            var file2s = await folderws.GetFileAsync("Tarefa");
            String text = await FileIO.ReadTextAsync(file2s);
            ChamaTile310x150(text);
        }

        private void ProgressoConversao(double _valor)
        {
            var progresso = Loader1.GetString("progresso");
            var salvo = Loader1.GetString("salvo");
            var salvoem = Loader1.GetString("salvoem");
            ProgressoPane.Visibility = Windows.UI.Xaml.Visibility.Visible;
            _ProgressValue.Value = _valor;
            _ProgressText.Text = $"{progresso} {_valor}%";
            if (_ProgressValue.Value == 100)
            {
                ProgressoPane.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                _ProgressValue.Value = 0;

                if (AtivarNotificacao.IsChecked == true && DesativarNotificacao.IsChecked == false)
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(salvo));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode($"{salvoem} \"{FileC1.Path}\""));
                    XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/StoreLogo.png");
                    ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));
                }
                else if (DesativarNotificacao.IsChecked == true && AtivarNotificacao.IsChecked == false)
                {
                    this.RootContextContainer.Children.Clear();
                    this.RootContextTitle.Text = salvo;
                    this.RootContextContainer.Children.Add(new TextBlock() { Text = salvoem, Style = this.Resources["RootContextTextStyle"] as Windows.UI.Xaml.Style });
                    this.RootContentContext.ShowAt(this);
                }
            }
        }

        private void ShowMainPopup()
        {
            this.RootContentContext.ShowAt(this);
        }

        private async void ConversaoCompletada()
        {
            Windows.Storage.Streams.IRandomAccessStream stream = await FileC1.OpenAsync(Windows.Storage.FileAccessMode.Read);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MediaView1.SetSource(stream, FileC1.ContentType);
             
            });
            visiveis();
            if (FileC1.FileType == ".mp3" || FileC1.FileType == ".m4a" || FileC1.FileType == ".wma" || FileC1.FileType == ".wav")
            {
                MusicProperties music = await FileC1.Properties.GetMusicPropertiesAsync();
                MediaView1.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioElementOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (mscTitulo.Text == "") music.Title = mscTitulo.PlaceholderText.ToString(); else music.Title = mscTitulo.Text.ToString();
                if (mscAlbum.Text == "") music.Album = mscAlbum.PlaceholderText.ToString(); else music.Album = mscAlbum.Text.ToString();
                if (mscArtistaAlbum.Text == "") music.AlbumArtist = mscArtistaAlbum.PlaceholderText.ToString(); else music.AlbumArtist = mscArtistaAlbum.Text.ToString();
                if (mscArtista.Text == "") music.Artist = mscArtista.Text.ToString(); else music.Artist = mscArtista.PlaceholderText.ToString();
                await music.SavePropertiesAsync();
            }
            else { AudioElementOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed; MediaView1.Visibility = Windows.UI.Xaml.Visibility.Visible; }
            var Date = Loader1.GetString("Convertido");
            var DateMantido = Loader1.GetString("mandito");
            if (File1.FileType == FileC1.FileType)
            {
                Criacao2.Text = $"{DateMantido} {FileC1.FileType}";
            }
            else
            {
                Criacao2.Text = $"{Date} {FileC1.FileType}";
            }

            var Conversao = Loader1.GetString("Conversao");
            var convertidoDe = Loader1.GetString("convertidoDe");
            var convertidoPara = Loader1.GetString("convertidoPara");
            var convertidoAs = Loader1.GetString("convertidoEm");
            StorageFolder lOCAL = ApplicationData.Current.LocalFolder;
            var pasta = await lOCAL.CreateFolderAsync("Tarefa", CreationCollisionOption.OpenIfExists);
            var arquivo = await pasta.CreateFileAsync("Tarefa", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(arquivo, $"{Conversao}\n{convertidoDe} {File1.FileType} {convertidoPara} {FileC1.FileType}\n{convertidoAs}\n{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
            var folderws = await lOCAL.GetFolderAsync("Tarefa");
            var file2s = await folderws.GetFileAsync("Tarefa");
            String text = await FileIO.ReadTextAsync(file2s);
            ChamaTile310x150(text);
            if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
            {
                MusicProperties music = await FileC1.Properties.GetMusicPropertiesAsync();
                MediaView1.Visibility = Windows.UI.Xaml.Visibility.Collapsed; AudioElementOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (mscTitulo.Text == "") music.Title = mscTitulo.PlaceholderText.ToString(); else music.Title = mscTitulo.Text.ToString();
                if (mscAlbum.Text == "") music.Album = mscAlbum.PlaceholderText.ToString(); else music.Album = mscAlbum.Text.ToString();
                if (mscArtistaAlbum.Text == "") music.AlbumArtist = mscArtistaAlbum.PlaceholderText.ToString(); else music.AlbumArtist = mscArtistaAlbum.Text.ToString();
                if (mscArtista.Text == "") music.Artist = mscArtista.Text.ToString(); else music.Artist = mscArtista.PlaceholderText.ToString();
                await music.SavePropertiesAsync();
            }
            File1 = FileC1;
            FileC1 = null;
            Criacao2.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void CancelarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                Cts1.Cancel();
                Cts1.Dispose();
                Cts1 = new CancellationTokenSource();
                visiveis();
                if (FileC1 != null)
                {
                    await FileC1.DeleteAsync();
                }

                var falha = Loader1.GetString("cancelado");
                var Motivofalha = Loader1.GetString("cancelamento");
                if (AtivarNotificacao.IsChecked == true && DesativarNotificacao.IsChecked == false)
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(falha));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode(Motivofalha));
                    XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/StoreLogo.png");
                    ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));
                }
                else if (DesativarNotificacao.IsChecked == true && AtivarNotificacao.IsChecked == false)
                {
                    this.RootContextContainer.Children.Clear();
                    this.RootContextTitle.Text = falha;
                    this.RootContextContainer.Children.Add(new TextBlock() { Text = Motivofalha, Style = this.Resources["RootContextTextStyle"] as Windows.UI.Xaml.Style });
                    this.RootContentContext.ShowAt(this);
                }
            }
            catch (Exception exception)
            {
                var Exfalha = Loader1.GetString("Exfalha");
                if (AtivarNotificacao.IsChecked == true && DesativarNotificacao.IsChecked == false)
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(Exfalha));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode(exception.Message));
                    XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/StoreLogo.png");
                    ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));
                }
                else if (DesativarNotificacao.IsChecked == true && AtivarNotificacao.IsChecked == false)
                {
                    this.RootContextContainer.Children.Clear();
                    this.RootContextTitle.Text = Exfalha;
                    this.RootContextContainer.Children.Add(new TextBlock() { Text = exception.Message, Style = this.Resources["RootContextTextStyle"] as Windows.UI.Xaml.Style });
                    this.RootContentContext.ShowAt(this);
                }
            }
            _ProgressValue.Value = 0;
            ProgressoPane.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            var Conversao = Loader1.GetString("Conversao");
            var convertidoDe = Loader1.GetString("convertidoDe");
            var convertidoPara = Loader1.GetString("convertidoPara");
            var convertidoCancelado = Loader1.GetString("convertidoCancelado");
            var convertidoAs = Loader1.GetString("convertidoAs");
            StorageFolder lOCAL = ApplicationData.Current.LocalFolder;
            var pasta = await lOCAL.CreateFolderAsync("Tarefa", CreationCollisionOption.OpenIfExists);
            var arquivo = await pasta.CreateFileAsync("Tarefa", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(arquivo, $"{Conversao}\n{convertidoDe} {File1.FileType} {convertidoPara} {FileC1.FileType}\n{convertidoCancelado}\n{convertidoAs} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
            var folderws = await lOCAL.GetFolderAsync("Tarefa");
            var file2s = await folderws.GetFileAsync("Tarefa");
            String text = await FileIO.ReadTextAsync(file2s);
            ChamaTile310x150(text);
        }

        private void ChamaTile310x150(string message)
        {
            if (Tile.IsOn)
            {
                var tile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150BlockAndText01);
                var tileAtributos = tile.GetElementsByTagName("text");
                tileAtributos[0].AppendChild(tile.CreateTextNode(message));
                var tileNotificar = new TileNotification(tile);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotificar);
            }
            else { TileUpdateManager.CreateTileUpdaterForApplication().Clear(); }
        }

        #endregion

        //Conversão para e video ou audio
        #region Audio para Video ? Video para Audio

        void VideoAudioConversao()
        {
            switch (TargetFormat.SelectedIndex)
            {
                case 0:
                    if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                    {
                        if (File1.FileType == ".mp4") AtualizarOMesmo(); else VideoParaVideo();
                    }
                    else if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                    {
                        AudioParaVideo();
                    }
                    break;
                case 1:
                    if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                    {
                        if (File1.FileType == ".avi") AtualizarOMesmo(); else VideoParaVideo();
                    }
                    else if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                    {
                        AudioParaVideo();
                    }
                    break;
                case 2:
                    if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                    {
                        if (File1.FileType == ".m4a") AtualizarOMesmo(); else AudioParaAudio();
                    }
                    else if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                    {
                        VideoParaAudio();
                    }
                    break;
                case 3:
                    if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                    {
                        if (File1.FileType == ".mp3") AtualizarOMesmo(); else AudioParaAudio();
                    }
                    else if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                    {
                        VideoParaAudio();
                    }
                    break;
                case 4:
                    if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                    {
                        if (File1.FileType == ".wav") AtualizarOMesmo(); else AudioParaAudio();
                    }
                    else if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                    {
                        VideoParaAudio();
                    }
                    break;
                case 5:
                    if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                    {
                        if (File1.FileType == ".wma") AtualizarOMesmo(); else AudioParaAudio();
                    }
                    else if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                    {
                        VideoParaAudio();
                    }
                    break;
                case 6:
                    if (File1.FileType == ".mp4" || File1.FileType == ".avi" || File1.FileType == ".wmv")
                    {
                        if (File1.FileType == ".wmv") AtualizarOMesmo(); else VideoParaVideo();
                    }
                    else if (File1.FileType == ".mp3" || File1.FileType == ".m4a" || File1.FileType == ".wma" || File1.FileType == ".wav")
                    {
                        AudioParaVideo();
                    }
                    break;
            }
        }

        void VideoParaAudio()
        {
            VideoToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            UpdateDoString.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoToAudio.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void AudioParaVideo()
        {
            VideoToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            UpdateDoString.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToVideo.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void VideoParaVideo()
        {
            AudioToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            UpdateDoString.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoToVideo.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void AudioParaAudio()
        {
            UpdateDoString.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToAudio.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void AtualizarOMesmo()
        {
            VideoToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoToVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioToAudio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            UpdateDoString.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        #endregion

        //Media em Play, Pause e Stop
        #region Midia play, pause e stop

        private void MediaView1_MediaOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (AutoPlay.IsOn) { MediaView1.Play(); PlayAndPauseIcon2.Symbol = Symbol.Pause; } else { MediaView1.Stop(); PlayAndPauseIcon2.Symbol = Symbol.Play; }
        }

        private void ButaoStop2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MediaView1.Stop();
            PlayAndPauseIcon2.Symbol = Symbol.Play;
        }

        private void ButaoPlayAndPause2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (MediaView1.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
            {
                MediaView1.Pause();
                PlayAndPauseIcon2.Symbol = Symbol.Play;
            }
            else
            {
                MediaView1.Play();
                PlayAndPauseIcon2.Symbol = Symbol.Pause;
            }
        }
        #endregion

        //Botão para mostrar os editores
        #region Botão mostrar mais e ocultar

        private void MostraEOcultar()
        {
            
            mscTag.Visibility = Windows.UI.Xaml.Visibility.Collapsed; _tag.Symbol = Symbol.Edit;
            VideoAudioSelectedIndex();
            var ocultar = Loader1.GetString("ocultar");
            var mostrar = Loader1.GetString("mostrar");
            if (OptionOfEdition.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                OptionOfEdition.Visibility = Windows.UI.Xaml.Visibility.Visible;
                MostrarOptions.Content = ocultar;
            }
            else
            {
                OptionOfEdition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                MostrarOptions.Content = mostrar;
            }

            if (TargetFormat.SelectedIndex == 0 || TargetFormat.SelectedIndex == 1 || TargetFormat.SelectedIndex == 6) { MostrarVideoEditor(); } else if (TargetFormat.SelectedIndex == 2 || TargetFormat.SelectedIndex == 3 || TargetFormat.SelectedIndex == 4 || TargetFormat.SelectedIndex == 6) { MostrarAudioEditor(); }
            //    switch (BoxEditors.SelectedIndex) { case 0: DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; break; case 1: ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; break; }
            if (TargetFormat.SelectedIndex == 2 || TargetFormat.SelectedIndex == 3 || TargetFormat.SelectedIndex == 4 || TargetFormat.SelectedIndex == 5) { TagEditor.Visibility = Windows.UI.Xaml.Visibility.Visible; } else { TagEditor.Visibility = Windows.UI.Xaml.Visibility.Collapsed; }
        }

        private void MostrarOptions_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MostraEOcultar();
        }

        #endregion

        //Area dos editores
        #region Area editores

        private void TagEditor_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (mscTag.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                //      switch (BoxEditors.SelectedIndex) { case 0: DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; break; case 1: DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; break; }

                mscTag.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                _tag.Symbol = Symbol.Edit;
            }
            else
            {
                ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                mscTag.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _tag.Symbol = Symbol.Back;
            }
        }

        private void Video_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _tag.Symbol = Symbol.Edit;
            VideoAudioSelectedIndex(); 
            MostrarVideoEditor();
            VideoAudioConversao();
            AudioOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
            TagEditor.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            mscTag.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //   switch (BoxEditors.SelectedIndex) { case 0: DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; break; case 1: ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; break; }
            Index();
         }

        void Index()
        {
            int A = TargetFormat.SelectedIndex;
            int B = VideoOption.SelectedIndex;
            int C = AudioOption.SelectedIndex;
            if (!Settings3.Values.ContainsKey(TargetFormat.Name)) Settings3.Values.Add(TargetFormat.Name, A); else Settings3.Values[TargetFormat.Name] = A;

            if (!Settings3.Values.ContainsKey(AudioOption.Name)) Settings3.Values.Add(AudioOption.Name, C); else Settings3.Values[AudioOption.Name] = C;

            if (!Settings3.Values.ContainsKey(VideoOption.Name)) Settings3.Values.Add(VideoOption.Name, B); else Settings3.Values[VideoOption.Name] = B;
        }

        private void Audio_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VideoAudioSelectedIndex();  
            MostrarAudioEditor();
            VideoAudioConversao();
            VideoOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
            TagEditor.Visibility = Windows.UI.Xaml.Visibility.Visible;
            mscTag.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //    switch (BoxEditors.SelectedIndex) { case 0: DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; break; case 1: ModernEdicao.Visibility = Windows.UI.Xaml.Visibility.Visible; DefaultEdicao.Visibility = Windows.UI.Xaml.Visibility.Collapsed; break; }
            Index();
        }

        private void Midias_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VideoAudioSelectedIndex();
            Index();
        }

        void MostrarAudioEditor()
        {
            VE.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VIE.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VideoEdition2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AudioEdition2.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
        void MostrarVideoEditor()
        {
            AudioEdition2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            VideoEdition2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            VE.Visibility = Windows.UI.Xaml.Visibility.Visible;
            VIE.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        #endregion

        //Fecha o painel central
        #region Fecha Painel Central
        private async void CloseRootContextBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.RootContentContext.Hide();
            await Task.Delay(300);
            this.RootContextContainer.Children.Clear();
            this.RootContextTitle.Text = string.Empty;
        }
        #endregion

        //Teclas do Menu Hamburger
        #region Teclas do Menu

        private void Comands_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var Comands = Loader1.GetString("comandos");
            var F1 = Loader1.GetString("F1");
            var F2 = Loader1.GetString("F2");
            var F3 = Loader1.GetString("F3");
            var F4 = Loader1.GetString("F4");
            //  var F5 = loader.GetString("F5");
            var F6 = Loader1.GetString("F6");
            var F7 = Loader1.GetString("F7");
            this.RootContextContainer.Children.Clear();
            this.RootContextWindow.MaxWidth = 450;
            this.RootContextTitle.Text = Comands;
            this.RootContextContainer.Children.Add(new TextBlock()
            {
                Text = $"{F1}\n{F2}\n{F3}\n{F4}\n{F6}\n{F7}",
                Style = this.Resources["RootContextTextStyle"] as Windows.UI.Xaml.Style
            });
            this.RootContentContext.ShowAt(this);
        }

        string Versao
        {
            get
            {
                var versaoAtual = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{versaoAtual.Major}.{versaoAtual.Minor}.{versaoAtual.Build}.{versaoAtual.Revision}";
            }
        }

        public ApplicationDataContainer Settings1 { get => Settings3; set => Settings3 = value; }
        public ResourceLoader Loader { get => Loader1; set => Loader1 = value; }
        public StorageFile File { get => File1; set => File1 = value; }
        public StorageFile FileC { get => FileC1; set => FileC1 = value; }
        public MediaEncodingProfile Profile { get => Profile1; set => Profile1 = value; }
        public string FileName { get => FileName1; set => FileName1 = value; }
        public string FileType { get => FileType1; set => FileType1 = value; }
        public string FileExtension { get => FileExtension1; set => FileExtension1 = value; }
        public CancellationTokenSource Cts { get => Cts1; set => Cts1 = value; }
        public MediaTranscoder Transcoder { get => Transcoder1; set => Transcoder1 = value; }
        public bool Mp41 { get => Mp42; set => Mp42 = value; }
        public bool Avi { get => Avi1; set => Avi1 = value; }
        public bool M4a { get => M4a1; set => M4a1 = value; }
        public bool Mp3 { get => Mp31; set => Mp31 = value; }
        public bool Wav { get => Wav1; set => Wav1 = value; }
        public bool Wma { get => Wma1; set => Wma1 = value; }
        public bool Wmv { get => Wmv1; set => Wmv1 = value; }
        public bool IsCelular1 { get => IsCelular2; set => IsCelular2 = value; }
        public ApplicationDataContainer Settings3 { get => _settings; set => _settings = value; }
        public ResourceLoader Loader1 { get => loader; set => loader = value; }
        public StorageFile File1 { get => _file; set => _file = value; }
        public StorageFile FileC1 { get => _fileC; set => _fileC = value; }
        public MediaEncodingProfile Profile1 { get => _Profile; set => _Profile = value; }
        public string FileName1 { get => _fileName; set => _fileName = value; }
        public string FileType1 { get => _fileType; set => _fileType = value; }
        public string FileExtension1 { get => _fileExtension; set => _fileExtension = value; }
        public CancellationTokenSource Cts1 { get => _cts; set => _cts = value; }
        public MediaTranscoder Transcoder1 { get => _Transcoder; set => _Transcoder = value; }
        public bool Mp42 { get => _mp4; set => _mp4 = value; }
        public bool Avi1 { get => _avi; set => _avi = value; }
        public bool M4a1 { get => _m4a; set => _m4a = value; }
        public bool Mp31 { get => _mp3; set => _mp3 = value; }
        public bool Wav1 { get => _wav; set => _wav = value; }
        public bool Wma1 { get => _wma; set => _wma = value; }
        public bool Wmv1 { get => _wmv; set => _wmv = value; }
        public bool IsCelular2 { get => IsCelular; set => IsCelular = value; }

        private void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var falha = Loader1.GetString("sobre");
            var Motivofalha = Loader1.GetString("sobreText");
            var Motivo = Loader1.GetString("versaoText");
            this.RootContextContainer.Children.Clear();
            this.RootContextWindow.MaxWidth = 350;
            this.RootContextTitle.Text = falha;
            this.RootContextContainer.Children.Add(new TextBlock() { Text = $"{Motivofalha}\n\n{Motivo} {Versao}", Style = this.Resources["RootContextTextStyle"] as Windows.UI.Xaml.Style });
            this.RootContentContext.ShowAt(this);
        }

        private async void FeedbackBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var app = Loader1.GetString("app");
            EmailMessage objEmail = new EmailMessage();
            objEmail.Subject = app; //Titulo do Feedback
            objEmail.To.Add(new EmailRecipient("igordutra2014@live.com")); //E-Mail que recebera o Feedback
            await EmailManager.ShowComposeNewEmailAsync(objEmail); //Pega todas as imformações e mande o feedback

        }

        private void Shared_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var sms = Loader1.GetString("sms");
            var app = Loader1.GetString("app");
            DataRequest request = args.Request;
            request.Data.SetText(sms);
            request.Data.Properties.Title = app;
            request.Data.Properties.Description = "Sent: " + DateTime.Now.ToString("dd/MM/yyyy");
            // request.Data.SetStorageItems(_fileC);
        }

        private async void ContactToBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContactPicker contactPicker = new ContactPicker(); //Abre a biblioteca de contatos do dispositivo
            contactPicker.CommitButtonText = "Selecione";
            contactPicker.SelectionMode = ContactSelectionMode.Fields;
            contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.Email);

            IList<Contact> contatos = await contactPicker.PickContactsAsync();
            if (contatos != null && contatos.Count > 0)
            {
                foreach (Contact contato in contatos) //Lista os contatos selecionados
                {
                    ContactStore contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);

                    if (contato != null)
                    {
                        Contact realcontatos = await contactStore.GetContactAsync(contato.Id);
                        var loader = new ResourceLoader();
                        var sms = loader.GetString("sms");
                        EnviarSMS(realcontatos, sms);
                    }
                }
            }

        }

        private async void EnviarSMS(Contact Contato, string Mensagem)
        {
            var MensagemChat = new Windows.ApplicationModel.Chat.ChatMessage();

            MensagemChat.Body = Mensagem; //Messagem = "Venha usar o NoteBlock disponivel para UWP e Android"

            var Celular = Contato.Phones.FirstOrDefault<ContactPhone>();
            if (Celular != null)
            {
                MensagemChat.Recipients.Add(Celular.Number);

                await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(MensagemChat);
            }
        }

        #endregion

        //Butão cnfigurações iniciar e voltar
        #region Butão Configurações, Inicio e Voltar

        private void Voltar_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GoSupport();
            Iniciar();
        }

        private void Settings_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GoSupport();
            RootGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            GridPainel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            GridSettings.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Settings.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Inicio.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Salvar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            BuscarFiles.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Separar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Separare2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Voltar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SwepararBuscarCancelar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //     Cancelar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Iniciar()
        {
            if (File1 == null)
            {
                RootGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                GridSettings.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                GridPainel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Settings.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Inicio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Salvar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                BuscarFiles.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Separar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Separare2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Voltar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                if (_ProgressValue.Value > 0)
                {
                    RootGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    GridPainel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    GridSettings.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Settings.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Inicio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    ProgressoPane.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Salvar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    BuscarFiles.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Separar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    OptionOfEdition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Separare2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Voltar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    SwepararBuscarCancelar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //        Cancelar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else if (_ProgressValue.Value == 0)
                {
                    RootGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    GridPainel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    OptionOfEdition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    GridSettings.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Settings.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Inicio.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    ProgressoPane.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    SwepararBuscarCancelar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //    Cancelar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Salvar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    BuscarFiles.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Separar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Separare2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Voltar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            ConfiguracoesUpdate();
        }

        #endregion

        //Contrles de cortes de midia
        #region Corte de edição
        private void InicioDoCorte_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (FinDocorte.Value > InicioDoCorte.Value) { FinDocorte.Value = InicioDoCorte.Value; }
            var _tempo = TimeSpan.FromMinutes(InicioDoCorte.Value);
            var _tempoString = _tempo.ToString(@"%m\:ss");
            var _tempo2 = TimeSpan.FromMinutes(FinDocorte.Value);
            var _tempoString2 = _tempo2.ToString(@"%m\:ss");
            IniMin.Text = _tempoString;
            FinMin.Text = _tempoString2;
        }

        private void FinDocorte_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (FinDocorte.Value > InicioDoCorte.Value) { FinDocorte.Value = InicioDoCorte.Value; }
            var _tempo = TimeSpan.FromMinutes(InicioDoCorte.Value);
            var _tempoString = _tempo.ToString(@"%m\:ss");
            var _tempo2 = TimeSpan.FromMinutes(FinDocorte.Value);
            var _tempoString2 = _tempo2.ToString(@"%m\:ss");
            IniMin.Text = _tempoString;
            FinMin.Text = _tempoString2;
        }
        #endregion

        //Maxinum de caracteries em TexBox
        #region Máximo de caractéries

        private void ResolucaoHeight_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (ResolucaoHeight.Text.Count() > 4)
            {
                ResolucaoHeight.Text = ResolucaoHeight.Text.Substring(0, 4);
                e.Handled = true;
                ResolucaoHeight.Select(4, 0);
            }
        }

        private void ResolucaoWidth_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (ResolucaoWidth.Text.Count() > 4)
            {
                ResolucaoWidth.Text = ResolucaoWidth.Text.Substring(0, 4);
                e.Handled = true;
                ResolucaoWidth.Select(4, 0);
            }
        }
        private void AudioBPS_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (AudioBPS.Text.Count() > 2)
            {
                AudioBPS.Text = AudioBPS.Text.Substring(0, 2);
                e.Handled = true;
                AudioBPS.Select(2, 0);
            }
        }

        private void AudioCC_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (AudioCC.Text.Count() > 1)
            {
                AudioCC.Text = AudioCC.Text.Substring(0, 1);
                e.Handled = true;
                AudioCC.Select(1, 0);
            }
        }

        private void AudioBR_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (AudioBR.Text.Count() > 6)
            {
                AudioBR.Text = AudioBR.Text.Substring(0, 6);
                e.Handled = true;
                AudioBR.Select(6, 0);
            }
        }

        private void AudioSR_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (AudioSR.Text.Count() > 5)
            {
                AudioSR.Text = AudioSR.Text.Substring(0, 5);
                e.Handled = true;
                AudioSR.Select(5, 0);
            }
        }

        private void VideoW_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (VideoW.Text.Count() > 4)
            {
                VideoW.Text = VideoW.Text.Substring(0, 4);
                e.Handled = true;
                VideoW.Select(4, 0);
            }
        }

        private void VideoH_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (VideoH.Text.Count() > 4)
            {
                VideoH.Text = VideoH.Text.Substring(0, 4);
                e.Handled = true;
                VideoH.Select(4, 0);
            }
        }

        private void VideoBR_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (VideoBR.Text.Count() > 7)
            {
                VideoBR.Text = VideoBR.Text.Substring(0, 7);
                e.Handled = true;
                VideoBR.Select(7, 0);
            }
        }

        private void VideoFR_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (VideoFR.Text.Count() > 2)
            {
                VideoFR.Text = VideoFR.Text.Substring(0, 2);
                e.Handled = true;
                VideoFR.Select(2, 0);
            }
        }
        #endregion

        //Midia em Geral
        #region Midia 

        private void MediaView1_MediaFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
        {
            MediaView1.Stop();
            PlayAndPauseIcon2.Symbol = Symbol.Play;
        }

        private void MediaView1_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MediaView1.Stop();
            PlayAndPauseIcon2.Symbol = Symbol.Play;
        }

        #region AudioAndVideo

        void VideoAudioSelectedIndex()
        {
            switch (TargetFormat.SelectedIndex)
            {
                case 0:
                    switch (VideoOption.SelectedIndex)
                    {
                          case 0:
                            MediaEncodingProfile defaultProfileHD1080p = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD1080p);
                            VideoW.PlaceholderText = defaultProfileHD1080p.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileHD1080p.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileHD1080p.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileHD1080p.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHD1080p.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileHD1080p.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileHD1080p.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileHD1080p.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileHD1080p.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileHD1080p.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHD1080p.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHD1080p.Audio.SampleRate.ToString();
                            break;
                        case 1:
                            MediaEncodingProfile defaultProfileHD720p = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD720p);
                            VideoW.PlaceholderText = defaultProfileHD720p.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileHD720p.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileHD720p.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileHD720p.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHD720p.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileHD720p.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileHD720p.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileHD720p.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileHD720p.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileHD720p.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHD720p.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHD720p.Audio.SampleRate.ToString();
                            break;
                        case 2:
                            MediaEncodingProfile defaultProfileWvga = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Wvga);
                            VideoW.PlaceholderText = defaultProfileWvga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileWvga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileWvga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileWvga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileWvga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileWvga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileWvga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileWvga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileWvga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileWvga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileWvga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileWvga.Audio.SampleRate.ToString();
                            break;
                        case 3:
                            MediaEncodingProfile defaultProfileVga = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Vga);
                            VideoW.PlaceholderText = defaultProfileVga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileVga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileVga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileVga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileVga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileVga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileVga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileVga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileVga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileVga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileVga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileVga.Audio.SampleRate.ToString();
                            break;
                        case 4:
                            MediaEncodingProfile defaultProfileQvga = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Qvga);
                            VideoW.PlaceholderText = defaultProfileQvga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileQvga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileQvga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileQvga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileQvga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileQvga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileQvga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileQvga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileQvga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileQvga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileQvga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileQvga.Audio.SampleRate.ToString();
                            break;
                    }
                    break;
                case 1:
                    switch (VideoOption.SelectedIndex)
                    {
                         case 0:
                            MediaEncodingProfile defaultProfileHD1080p = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.HD1080p);
                            VideoW.PlaceholderText = defaultProfileHD1080p.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileHD1080p.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileHD1080p.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileHD1080p.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHD1080p.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileHD1080p.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileHD1080p.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileHD1080p.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileHD1080p.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileHD1080p.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHD1080p.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHD1080p.Audio.SampleRate.ToString();
                            break;
                        case 1:
                            MediaEncodingProfile defaultProfileHD720p = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.HD720p);
                            VideoW.PlaceholderText = defaultProfileHD720p.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileHD720p.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileHD720p.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileHD720p.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHD720p.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileHD720p.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileHD720p.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileHD720p.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileHD720p.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileHD720p.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHD720p.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHD720p.Audio.SampleRate.ToString();
                            break;
                        case 2:
                            MediaEncodingProfile defaultProfileWvga = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Wvga);
                            VideoW.PlaceholderText = defaultProfileWvga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileWvga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileWvga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileWvga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileWvga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileWvga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileWvga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileWvga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileWvga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileWvga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileWvga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileWvga.Audio.SampleRate.ToString();
                            break;
                        case 3:
                            MediaEncodingProfile defaultProfileVga = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Vga);
                            VideoW.PlaceholderText = defaultProfileVga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileVga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileVga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileVga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileVga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileVga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileVga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileVga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileVga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileVga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileVga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileVga.Audio.SampleRate.ToString();
                            break;
                        case 4:
                            MediaEncodingProfile defaultProfileQvga = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Qvga);
                            VideoW.PlaceholderText = defaultProfileQvga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileQvga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileQvga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileQvga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileQvga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileQvga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileQvga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileQvga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileQvga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileQvga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileQvga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileQvga.Audio.SampleRate.ToString();
                            break;
                    }
                    break;
                case 2:
                    switch (AudioOption.SelectedIndex)
                    {
                        case 0:
                            MediaEncodingProfile defaultProfileLow = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Low);
                            AudioCC.PlaceholderText = defaultProfileLow.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileLow.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileLow.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileLow.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileLow.Audio.BitsPerSample;
                            break;
                        case 1:
                            MediaEncodingProfile defaultProfileMedium = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Medium);
                            AudioCC.PlaceholderText = defaultProfileMedium.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileMedium.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileMedium.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileMedium.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileMedium.Audio.BitsPerSample;
                            break;
                        case 2:
                            MediaEncodingProfile defaultProfileHigh = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.High);
                            AudioCC.PlaceholderText = defaultProfileHigh.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHigh.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHigh.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHigh.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileHigh.Audio.BitsPerSample;
                            break;
                    }
                    break;
                case 3:
                    switch (AudioOption.SelectedIndex)
                    {
                        case 0:
                            MediaEncodingProfile defaultProfileLow = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Low);
                            AudioCC.PlaceholderText = defaultProfileLow.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileLow.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileLow.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileLow.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileLow.Audio.BitsPerSample;
                            break;
                        case 1:
                            MediaEncodingProfile defaultProfileMedium = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Medium);
                            AudioCC.PlaceholderText = defaultProfileMedium.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileMedium.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileMedium.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileMedium.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileMedium.Audio.BitsPerSample;
                            break;
                        case 2:
                            MediaEncodingProfile defaultProfileHigh = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High);
                            AudioCC.PlaceholderText = defaultProfileHigh.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHigh.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHigh.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHigh.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileHigh.Audio.BitsPerSample;
                            break;
                    }
                    break;
                case 4:
                    switch (AudioOption.SelectedIndex)
                    {
                          case 0:
                            MediaEncodingProfile defaultProfileLow = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);
                            AudioCC.PlaceholderText = defaultProfileLow.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileLow.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileLow.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileLow.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileLow.Audio.BitsPerSample;
                            break;
                        case 1:
                            MediaEncodingProfile defaultProfileMedium = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);
                            AudioCC.PlaceholderText = defaultProfileMedium.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileMedium.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileMedium.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileMedium.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileMedium.Audio.BitsPerSample;
                            break;
                        case 2:
                            MediaEncodingProfile defaultProfileHigh = MediaEncodingProfile.CreateWav(AudioEncodingQuality.High);
                            AudioCC.PlaceholderText = defaultProfileHigh.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHigh.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHigh.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHigh.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileHigh.Audio.BitsPerSample;
                            break;
                    }
                    break;
                case 5:
                    switch (AudioOption.SelectedIndex)
                    {
                        case 0:
                            MediaEncodingProfile defaultProfileLow = MediaEncodingProfile.CreateWma(AudioEncodingQuality.Low);
                            AudioCC.PlaceholderText = defaultProfileLow.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileLow.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileLow.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileLow.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileLow.Audio.BitsPerSample;
                            break;
                        case 1:
                            MediaEncodingProfile defaultProfileMedium = MediaEncodingProfile.CreateWma(AudioEncodingQuality.Medium);
                            AudioCC.PlaceholderText = defaultProfileMedium.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileMedium.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileMedium.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileMedium.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileMedium.Audio.BitsPerSample;
                            break;
                        case 2:
                            MediaEncodingProfile defaultProfileHigh = MediaEncodingProfile.CreateWma(AudioEncodingQuality.High);
                            AudioCC.PlaceholderText = defaultProfileHigh.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHigh.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHigh.Audio.SampleRate.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHigh.Audio.BitsPerSample.ToString();
                            AudioBPS2.Value = defaultProfileHigh.Audio.BitsPerSample;
                            break;
                    }
                    break;
                case 6:
                    switch (VideoOption.SelectedIndex)
                    {
                          case 0:
                            MediaEncodingProfile defaultProfileHD1080p = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.HD1080p);
                            VideoW.PlaceholderText = defaultProfileHD1080p.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileHD1080p.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileHD1080p.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileHD1080p.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHD1080p.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileHD1080p.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileHD1080p.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileHD1080p.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileHD1080p.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileHD1080p.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHD1080p.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHD1080p.Audio.SampleRate.ToString();
                            break;
                        case 1:
                            MediaEncodingProfile defaultProfileHD720p = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.HD720p);
                            VideoW.PlaceholderText = defaultProfileHD720p.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileHD720p.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileHD720p.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileHD720p.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileHD720p.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileHD720p.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileHD720p.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileHD720p.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileHD720p.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileHD720p.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileHD720p.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileHD720p.Audio.SampleRate.ToString();
                            break;
                        case 2:
                            MediaEncodingProfile defaultProfileWvga = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Wvga);
                            VideoW.PlaceholderText = defaultProfileWvga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileWvga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileWvga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileWvga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileWvga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileWvga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileWvga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileWvga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileWvga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileWvga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileWvga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileWvga.Audio.SampleRate.ToString();
                            break;
                        case 3:
                            MediaEncodingProfile defaultProfileVga = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Vga);
                            VideoW.PlaceholderText = defaultProfileVga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileVga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileVga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileVga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileVga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileVga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileVga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileVga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileVga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileVga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileVga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileVga.Audio.SampleRate.ToString();
                            break;
                        case 4:
                            MediaEncodingProfile defaultProfileQvga = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Qvga);
                            VideoW.PlaceholderText = defaultProfileQvga.Video.Width.ToString();
                            VideoH.PlaceholderText = defaultProfileQvga.Video.Height.ToString();
                            VideoBR.PlaceholderText = defaultProfileQvga.Video.Bitrate.ToString();
                            VideoFR.PlaceholderText = defaultProfileQvga.Video.FrameRate.Numerator.ToString();
                            AudioBPS.PlaceholderText = defaultProfileQvga.Audio.BitsPerSample.ToString();
                            VideoFR2.Value = defaultProfileQvga.Video.FrameRate.Numerator;
                            AudioBPS2.Value = defaultProfileQvga.Audio.BitsPerSample;
                            ResolucaoHeight.PlaceholderText = defaultProfileQvga.Video.Height.ToString();
                            ResolucaoWidth.PlaceholderText = defaultProfileQvga.Video.Width.ToString();
                            AudioCC.PlaceholderText = defaultProfileQvga.Audio.ChannelCount.ToString();
                            AudioBR.PlaceholderText = defaultProfileQvga.Audio.Bitrate.ToString();
                            AudioSR.PlaceholderText = defaultProfileQvga.Audio.SampleRate.ToString();
                            break;
                    }
                    break;
            }
        }

        #endregion

        #endregion

        //Area de Apoio/Ajuda
        private void Apoio_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        //Menu Hamburger
        private void MenuHanburger_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            OpenMenu.IsPaneOpen = !OpenMenu.IsPaneOpen;
        }
    }
}
#endregion
#endregion