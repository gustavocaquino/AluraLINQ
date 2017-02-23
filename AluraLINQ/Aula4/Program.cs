using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ZXing;

namespace Aula4
{
    using static System.Console;

    class Program
    {
        #region Lista de Produtos
        static readonly IEnumerable<Produto> _listaProdutos = new List<Produto>
        {
            new Produto("PlayStation 4 Pro", @"http://meusite.com/playstation4"),
            new Produto("XBox One Slim", @"http://meusite.com/xboxone")
        };
        #endregion

        static void Main(string[] args)
        {
            const string path = "imagens";
            var cronometro = new Stopwatch();

            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 100,
                    Height = 100
                }
            };

            CheckIfPathExists(path);

            cronometro.Start();

            var codigos = Enumerable.Repeat(_listaProdutos, 100).SelectMany(x => x).ToList().AsParallel().Select(x => new
            {
                Arquivo = $"{path}\\{x.Id}.{ImageFormat.Jpeg}",
                Imagem = barcodeWriter.Write($"{x.Url}/{x.Id.ToString()}")
            });

            cronometro.Stop();

            WriteLine($"{codigos.Count()} imagens geradas em {cronometro.ElapsedMilliseconds / 1000.0} segundos.");

            cronometro.Restart();

            foreach (var codigo in codigos)
                codigo.Imagem.Save(codigo.Arquivo, ImageFormat.Jpeg);

            cronometro.Stop();

            WriteLine($"ForEach: {codigos.Count()} imagens gravadas em {cronometro.ElapsedMilliseconds / 1000.0} segundos.");

            cronometro.Restart();

            codigos.ForAll(x => x.Imagem.Save(x.Arquivo, ImageFormat.Jpeg));

            cronometro.Stop();

            WriteLine($"ForAll: {codigos.Count()} imagens gravadas em {cronometro.ElapsedMilliseconds / 1000.0} segundos.");

            ReadKey();
        }

        private static void CheckIfPathExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
