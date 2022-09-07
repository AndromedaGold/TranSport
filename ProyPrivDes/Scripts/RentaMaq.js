listar();
function listar() {
    $.get("RentaMaq/listarRenta", function (data) {
        crearListado(["ID", "Nombre", "Color", "Marca", "Modelo", "Precio", "Fecha Renta"], data)
    });
}

$("#datepickerRentaMaq").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

var btnBuscar = document.getElementById("btnBuscar");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("RentaMaq/buscarRentaMaq/?nombre=" + nombre, function (data) {
        crearListado(["ID", "Nombre", "Color", "Marca", "Modelo", "Precio", "Fecha Renta"], data)
    })
}

var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    $.get("RentaMaq/listarRenta", function (data) {
        crearListado(["ID", "Nombre", "Color", "Marca", "Modelo", "Precio", "Fecha Renta"], data)
    });
    document.getElementById("txtnombre").value = "";
}

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-rentamaq' class='table'>";
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
    $("#tabla-rentamaq").dataTable({
        searching: false
    });
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

function abrirModal(id) {
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++){
        controlesObligatorios[i].parentNode.classList.remove("error");
    }
    if (id == 0) {
        borrarDatos();
    } else {
        $.get("RentaMaq/mostrarInfo/?id=" + id, function (data) {
            document.getElementById("txtId").value = data[0].Id;
            document.getElementById("txtNombre").value = data[0].Nombre;
            document.getElementById("txtColor").value = data[0].Color;
            document.getElementById("txtMarca").value = data[0].Marca;
            document.getElementById("txtModelo").value = data[0].Modelo;
            document.getElementById("txtPrecio").value = data[0].Precio;
            document.getElementById("cboxCombustible").value = data[0].CombustibleId;
            document.getElementById("cboxTipoMaq").value = data[0].TipoMaqId;
            document.getElementById("datepickerRentaMaq").value = data[0].FechaRenta;
            document.getElementById("txtHabilitado").value = data[0].Habilitado;
        });
    }
}

Combos();
function Combos() {
    $.get("RentaMaq/listarCombustible", function (data) {
        llenarCombo(data, document.getElementById("cboxCombustible"), true)

    });

    $.get("RentaMaq/listarTipoMaq", function (data) {
        llenarCombo(data, document.getElementById("cboxTipoMaq"), true)

    });
}

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

function eliminar(id) {
    if (confirm("¿Desea eliminar el dato?") == 1) {
        $.get("RentaMaq/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrio un error");
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
        var id = document.getElementById("txtId").value;
        var nombre = document.getElementById("txtNombre").value;
        var color = document.getElementById("txtColor").value;
        var marca = document.getElementById("txtMarca").value;
        var modelo = document.getElementById("txtModelo").value;
        var precio = document.getElementById("txtPrecio").value;
        var combustible = document.getElementById("cboxCombustible").value;
        var tipo = document.getElementById("cboxTipoMaq").value;
        var fecha = document.getElementById("datepickerRentaMaq").value;
        var habilitado = document.getElementById("txtHabilitado").value;

        frm.append("Id", id);
        frm.append("Nombre", nombre);
        frm.append("Color", color);
        frm.append("Marca", marca);
        frm.append("Modelo", modelo);
        frm.append("Precio", precio);
        frm.append("CombustibleId", combustible);
        frm.append("TipoMaqId", tipo);
        frm.append("FechaRenta", fecha);
        frm.append("Habilitado", habilitado);

        if (confirm("¿Desea guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "RentaMaq/guardar",
                data:frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("El dato se guardo correctamente");
                        listar();
                        document.getElementById("btnCancelar").click();
                    } else {
                        if (data == -1) {
                            alert("El dato ya exsite");
                        }
                        alert("Ocurrio un error");
                    }
                }
            });
        }
    }
}

$(document).ready(function () {

    //Boton ver reporte
    $("#btnMostrarPDF").on("click", function () {
        $("#RptRentaMaq").attr("src", "RentaMaq/RptRentaMaq").load();
    });

    //Boton descargar reporte
    $("#btnDescargarPDF").on("click", function () {
        window.open("RentaMaq/RptRentaMaq")
    });

});
