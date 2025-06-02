using System.Collections.ObjectModel;
using System.Diagnostics;
using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Tempo> lista = new ObservableCollection<Tempo>();

        

        public MainPage()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista;
        }

        

        private async void GetCidade(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da Cidade", ex.Message, "OK");
            }

        }
        

        public async void Previsao_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {                    
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        t.data =DateTime.Now;

                        /*dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp Máx: {t.temp_max} \n" +
                                         $"Temp Min: {t.temp_min} \n";

                        lbl_res.Text = dados_previsao;
                        */

                        await App.Db.Insert(t);

                        lista.Clear();
                        App.Db.GetAll().Result.ForEach(i => lista.Add(i));
                    }
                    else
                    {

                        lbl_res.Text = "Sem dados de Previsão";
                    }// fecha if t=null
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }// fecha if string is null or empty
            }
            catch (Exception ex)
            {
                Debug.WriteLine("-------------------------------------------------");
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine("-------------------------------------------------");

                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
        private async void lst_produtos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear();

                List<Tempo> tmp = await App.Db.GetAll();

                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");

            }
            finally
            {
                lst_produtos.IsRefreshing = false;
            }
        }
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecinado = sender as MenuItem;

                Tempo p = selecinado.BindingContext as Tempo;

                bool confirm = await DisplayAlert(
                    "Tem Certeza?", $"Remover {p.description}?", "Sim", "Não");

                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    lista.Remove(p);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
        protected async override void OnAppearing()
        {
            try
            {
                lista.Clear();

                List<Tempo> tmp = await App.Db.GetAll();

                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }

}
