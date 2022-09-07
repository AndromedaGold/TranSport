var btnIngresar = document.getElementById("btnIngresar");
btnIngresar.onclick = function () {
    var usuario = document.getElementById("txtUsuario").value;
    var contraseña = document.getElementById("txtContraseña").value;

    if (usuario == "") {
        alert("Ingrese un usuario");
        return;
    }

    if (contraseña == "") {
        alert("Ingrese una contraseña");
        return;
    }

    $.get("Login/validarUsuario/?usuario=" + usuario + "&contraseña=" + contraseña, function (data) {

        if (data == 1) {
            document.location.href = "PaginaPrincipal/Index";
        } else {
            alert("Usuario o contraseña incorrecta");
        }
    })
}