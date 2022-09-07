function listar() {
    $.get("Coordinador/listarCoordinador", function (data) {
        crearListado(["ID", "Nombre", "Apellido", "Telefono", "Fecha de Alta"], data);
    })
}
listar();

$("#datepickerFechaAlta").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

$.get("Coordinador/listarEstado", function (data) {
    llenarCombo(data, document.getElementById("cboEstado"), true)
    llenarCombo(data, document.getElementById("cboxEstado"), true)

});

function llenarCombo(data, control, primerElemento) {
    var contenido = "";
    if (primerElemento == true) {
        contenido += "<option value=''>--Seleccione--</option>";
    }
    for (var i = 0; i < data.length; i++) {
        contenido += "<option value='" + data[i].ID + "'>";
        contenido += data[i].Descripcion;
        contenido += "</option>";
    }
    control.innerHTML = contenido;
}

var btnBuscar = document.getElementById("btnBuscar");
btnBuscar.onclick = function () {
    var idestado = document.getElementById("cboEstado").value;
    if (idestado == "") {
        listar();
    } else
        $.get("Coordinador/filtrarEstado/?idEstado=" + idestado, function (data) {
            crearListado(["ID", "Nombre", "Apellido", "Telefono", "Fecha de Alta"], data);
        });
}

var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    listar();
}

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-coordinador' class='table'>";
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
    $("#tabla-coordinador").dataTable({
        searching: false
    });
}

function abrirModal(id) {
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }
    if (id == 0) {
        borrarDatos();
    } else {
        $.get("Coordinador/mostrarInfo/?id=" + id, function (data) {
            document.getElementById("txtId").value = data[0].Id;
            document.getElementById("txtNombre").value = data[0].Nombre;
            document.getElementById("txtApellido").value = data[0].Apellido;
            document.getElementById("txtTelefono").value = data[0].Telefono;
            document.getElementById("datepickerFechaAlta").value = data[0].FechaAlt;
            document.getElementById("cboxEstado").value = data[0].EstadoId;
        });
    }
}

function borrarDatos() {
    var controles = document.getElementsByClassName("borrar");
    var ncontroles = controles.length;
    for (var i = 0; i < ncontroles; i++) {
        controles[i].value = "";
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

function eliminar(id) {
    if (confirm("¿Desea eliminar el dato?") == 1) {
        $.get("Coordinador/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrio un error");
            } else {
                alert("Se eliminó correctamente");
                listar();
            }
        })
    }
}

function Agregar() {
    if (datosObligatorios() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtId").value;
        var nombre = document.getElementById("txtNombre").value;
        var apellido = document.getElementById("txtApellido").value;
        var telefono = document.getElementById("txtTelefono").value;
        var fecha = document.getElementById("datepickerFechaAlta").value;
        var estado = document.getElementById("cboxEstado").value;

        frm.append("Id", id);
        frm.append("Nombre", nombre);
        frm.append("Apellido", apellido);
        frm.append("Telefono", telefono);
        frm.append("FechaAlt", fecha);
        frm.append("EstadoId", estado);

        if (confirm("¿Desea guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "Coordinador/guardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("El dato se guardo corectamente");
                        listar();
                        document.getElementById("btnCancelar").click();
                    } else {
                        if (data == -1) {
                            alert("El dato ya existe");
                        }
                        alert("Ocurrio un error");
                    }
                }
            })
        }
    }
}