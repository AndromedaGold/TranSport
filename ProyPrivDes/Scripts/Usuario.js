listar();
function listar() {

    $.get("Usuario/listarUsuarios", function (data) {
        crearListado(["Id Usuario", "Nombre Completo", "Usuario", "Nombre Rol", "Tipo"], data);
    })
    $.get("Usuario/listarRol", function (data) {
        llenarCombo(data, document.getElementById("cboRol"), true);
    })
    $.get("Usuario/listarPersonas", function (data) {
        llenarCombo(data, document.getElementById("cboPersona"), true);
    })
}

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-usuario' class='table'>";
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < arrayColumnas.length; i++) {
        contenido += "<td>";
        contenido += arrayColumnas[i];
        contenido += "</td>";
    }
    contenido += "<td>Acciones</td>"
    contenido += "</tr>";
    contenido += "</thead>";

    var llaves = Object.keys(data[0]);
    contenido += "<tbody>";
    for (var j = 0; j < data.length; j++) {
        contenido += "<tr>";
        for (var k = 0; k < llaves.length; k++) {
            var valorLlaves = llaves[k];
            contenido += "<td>";
            contenido += data[j][valorLlaves];
            contenido += "</td>";
        }
        var llaveID = llaves[0];
        contenido += "<td>";
        contenido += "<button class='btn btn-primary' onclick='abrirModal(" + data[j][llaveID] + ")' data-toggle='modal' data-target='#myModal'><i class='glyphicon glyphicon-edit' ></i></button> ";
        contenido += " <button class='btn btn-danger' onclick='eliminar(" + data[j][llaveID] + ")'><i class='glyphicon glyphicon-trash' ></i></button>"
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("tabla").innerHTML = contenido;
    $("#tabla-usuario").dataTable({
        searching: false
    });
}

function llenarCombo(data, control, primerElemento) {
    var contenido = "";
    if (primerElemento == true) {
        contenido += "<option value=''>--Seleccione--</option>";
    }
    for (var i = 0; i < data.length; i++) {
        contenido += "<option value='" + data[i].ID + "'>";
        contenido += data[i].Nombre;
        contenido += "</option>";
    }
    control.innerHTML = contenido;
}

function abrirModal(id) {
    var controlesObligatorio = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorio.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorio[i].parentNode.classList.remove("error");
    }
    if (id == 0) {
        document.getElementById("lblContraseña").style.display = "block";
        document.getElementById("txtContraseña").style.display = "block";
        document.getElementById("lblPersona").style.display = "block";
        document.getElementById("cboPersona").style.display = "block";
        borrarDatos();
    } else {
        document.getElementById("lblContraseña").style.display = "none";
        document.getElementById("txtContraseña").style.display = "none";
        document.getElementById("lblPersona").style.display = "none";
        document.getElementById("cboPersona").style.display = "none";
        document.getElementById("txtContraseña").value = "1";
        document.getElementById("cboPersona").value = "2";

        $.get("Usuario/recuperarDatos/?idusuario=" + id, function (data) {
            document.getElementById("txtIDusuario").value = data.IdUsuario;
            document.getElementById("txtNombre").value = data.NombreUsuario;
            document.getElementById("cboRol").value = data.IdRol;

        })
    }
}

function datosObligatorios() {
    var exito = true;
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        if (controlesObligatorios[i].value == "") {
            exito = false;
            controlesObligatorios[i].parentNode.classList.add("error");
        } else {
            controlesObligatorios[i].parentNode.classList.remove("error");
        }
    }
    return exito;
}

function borrarDatos() {
    var controles = document.getElementsByClassName("borrar");
    var ncontroles = controles.length;
    for (var i = 0; i < ncontroles; i++) {
        controles[i].value = "";
    }
}

function eliminar(id) {
    if (confirm("¿Desea eliminar el dato?") == 1) {
        $.get("Usuario/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrió un error");
            } else {
                alert("Se elimino correctamente");
                listar();
            }
        });
    }
}

function Agregar() {
    if (datosObligatorios() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtIDusuario").value;
        var nombre = document.getElementById("txtNombre").value;
        var contra = document.getElementById("txtContraseña").value;
        var persona = document.getElementById("cboPersona").value;
        var rol = document.getElementById("cboRol").value;
        var nombrePersona = document.getElementById("cboPersona").options[document.getElementById("cboPersona").selectedIndex].text;

        frm.append("IdUsuARIO", id);
        frm.append("NombreUsuario", nombre);
        frm.append("Contra", contra);
        frm.append("Id", persona);
        frm.append("IdRol", rol);
        frm.append("nombreCompleto", nombrePersona);
        frm.append("Habilitado", 1);

        if (confirm("¿Desea guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "Usuario/guardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("Se guardó correctamente");
                        document.getElementById("btnCancelar").click();
                        listar();
                    } else {
                        alert("Ocurrió un error");
                    }
                }
            });
        }
    }
}