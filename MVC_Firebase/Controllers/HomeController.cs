using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using MVC_Firebase.Models;
using System.Diagnostics;

namespace MVC_Firebase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> SubirArchivo(IFormFile archivo)
        {
            //Leemos al archivo subido
            Stream archivoASubir = archivo.OpenReadStream();

            //Configuramos la conexion hacia FireBase
            string email = "fernando.gomez@catolica.edu.sv";  //Correo para autenticar en Firebase
            string clave = "UNICAES";  //Contraseña establecida en la autenticar en FireBase
            string ruta = "mvcfirebase-112f1.appspot.com";  //URL donde se guardan los archivos
            string api_key = "AIzaSyDaXrwQqeOcFjBmS_hq5Rvr3uQgBZJHkTk";  //API_KEY identificador del proyecto en FireBase

            //Autentificacion a FireBase
            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));  //Indicamos la api_key del proyecto
            var autenticarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);  //Indicamos el email y la clave

            var cancellation = new CancellationTokenSource();  //Token de concelacion
            var tokenUser = autenticarFireBase.FirebaseToken;  //Token de usuario

            //Configuracion del envio al storage de FireBase
            var tareaCargarArchivo = new FirebaseStorage(ruta,
                                                        new FirebaseStorageOptions
                                                        {
                                                            AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                                                            ThrowOnCancel = true
                                                        }
                                                        ).Child("Archivos")
                                                        .Child(archivo.FileName)
                                                        .PutAsync(archivoASubir, cancellation.Token);

            var urlArchivoCargado = await tareaCargarArchivo;

            return RedirectToAction("Index");

        }
    }
}
