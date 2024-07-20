using System.Diagnostics;

namespace IntroAsync
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBaseParelelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecucion(destinoBaseParelelo, destinoBaseSecuencial);

            Console.WriteLine("inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            //Parte Secuencial
            var sw = new Stopwatch();
            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }

            Console.WriteLine("Paralelo - duracion en segundos: {0}",
               sw.ElapsedMilliseconds / 1000.00);

            sw.Reset();
            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParelelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);

            sw.Stop();

            var duracion = $"El programa se ejecuto en {sw.ElapsedMilliseconds / 1000.0} segundos";
            Console.WriteLine(duracion);

         
            pictureBox1.Visible = false;
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(

                    new Imagen()
                    {
                        Nombre = $"Brasil {i}.jpg",
                        URL = "https://img.freepik.com/fotos-premium/pelota-futbol-brasil_103577-3347.jpg?w=740"
                    });
                imagenes.Add(
                      new Imagen()
                      {
                          Nombre = $"Real Madrid {i}.jpg",
                          URL = "https://imageio.forbes.com/specials-images/imageserve/65ab8d96ee06c40dad0e2cc9/Luka-Modric-will-stay-at-Real-Madrid-for-at-least-another-year-/960x0.jpg?format=jpg&width=960"
                      });

                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"The Neighbourhood {i}.jpg",
                        URL = "https://i.iheart.com/v3/catalog/artist/31307?ops=fit(720%2C720)"
                    });
            }
            return imagenes;

        }
        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoParalelo,
            string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoParalelo))
            {
                Directory.CreateDirectory(destinoParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoParalelo);
        }


        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            using (var httpClient = new HttpClient())
            {
                var respuesta = await httpClient.GetAsync(imagen.URL);

                if (respuesta.IsSuccessStatusCode)
                {
                    var contenido = await respuesta.Content.ReadAsByteArrayAsync();

                    using (var ms = new MemoryStream(contenido))
                    {
                        using (var bitmap = new Bitmap(ms))
                        {
                            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            var destino = Path.Combine(directorio, imagen.Nombre);
                            bitmap.Save(destino);
                        }
                    }
                }
            }
        }


        private async Task<String> ProcesamientoLargo()
        {
            await Task.Delay(5000);
            return "Tek Ni";
        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso A finalizado");
        }

        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B finalizado");
        }

        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C finalizado");
        }
    }

}
    
