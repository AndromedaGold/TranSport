function listar() {
    $.get("RolPagina/listarRol", function (data) {
        crearListado(["ID Rol", "Nombre", "Descripcion"], data);
    })
}

listar();

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-rolpagina' class='table'>";
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < arrayColumnas.length; i++) {
        contenido += "<td>";
        contenido += arrayColumnas[i];
        contenido += "</td>";
    }
    contenido += "<td>Accion</td>"
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
        
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("tabla").innerHTML = contenido;
    $("#tabla-rolpagina").dataTable({
        searching: false
    });
}

var idrol;
function abrirModal(id) {
    idrol = id;
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }

    $.get("RolPagina/listarPaginas", function (data) {
        var contenido = "<tbody>";
        for (var i = 0; i < data.length; i++) {
            contenido += "<tr>";
            contenido += "<td>";
            contenido += "<input class='checkbox' type='checkbox' id='" + data[i].IdPagina + "' />"
            contenido += "</td>";
            contenido += "<td>";
            contenido += data[i].Mensaje;
            contenido += "</td>";
            contenido += "</tr>";
        }
        contenido += "</tbody>";
        document.getElementById("tblPagina").innerHTML = contenido;
        if (id > 0) {
            obtenerRolPagina();
        }
    })
    if (id == 0) {
        borrarDatos();
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

function Agregar() {
    if (datosObligatorios() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtIDrol").value;
        var rol = document.getElementById("txtNombreRol").value;
        var desc = document.getElementById("txtDescripcion").value;

        frm.append("IdRol", id);
        frm.append("Nombre", rol);
        frm.append("Descripcion", desc);
        frm.append("Habilitado", 1);

        var checkbox = document.getElementsByClassName("checkbox");
        var ncheckbox = checkbox.length;
        var dataEnviar = "";

        for (var i = 0; i < ncheckbox; i++) {
            if (checkbox[i].checked == true) {
                dataEnviar += checkbox[i].id;
                dataEnviar += "$";
            }
        }
        dataEnviar = dataEnviar.substring(0, dataEnviar.length - 1);
        frm.append("dataEnviar", dataEnviar);

        if (confirm("¿Desea guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "RolPagina/guardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 0) {
                        alert("Ocurrió un error");
                    } else {
                        alert("Se guardó correctamente");
                        document.getElementById("btnCancelar").click();
                        listar();
                    }
                }
            });
        }
    }
}

function obtenerRolPagina() {
    $.get("RolPagina/obtenerRolPagina/?oRol=" + idrol, function (data) {
        var nregistros = data.length;
        for (var i = 0; i < nregistros; i++) {
            if (data[i].Habilitado == 1) {
                document.getElementById(data[i].IdPagina).checked = true;
            }
        }
    })

    $.get("RolPagina/obtenerRol/?oRol=" + idrol, function (data) {
        document.getElementById("txtIDrol").value = data.IdRol;
        document.getElementById("txtNombreRol").value = data.Nombre;
        document.getElementById("txtDescripcion").value = data.Descripcion;
    })
}

