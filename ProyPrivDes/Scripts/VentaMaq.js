$("#datepickerVentaMaq").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

$(document).ready(function () {

    //Boton ver reporte
    $("#btnMostrarPDF").on("click", function () {
        $("#RptVentaMaq").attr("src", "VentaMaq/RptVentaMaq").load();
    });

    //Boton descargar reporte
    $("#btnDescargarPDF").on("click", function () {
        window.open("VentaMaq/RptVentaMaq")
    });

});

listar();

function listar() {
    $.get("VentaMaq/listarVentaMaq", function (data) {
        crearListado(["ID", "Nombre", "Color", "Peso", "Marca", "Modelo", "Precio","Fecha Venta"], data);
    })
}

var btnBuscar = document.getElementById("btnBuscar");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("VentaMaq/buscarVentaMaq/?nombre=" + nombre, function (data) {
        crearListado(["ID", "Nombre", "Color", "Peso", "Marca", "Modelo", "Precio", "Fecha Venta"], data);
    })
}

var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    $.get("VentaMaq/listarVentaMaq", function (data) {
        crearListado(["ID", "Nombre", "Color", "Peso", "Marca", "Modelo", "Precio", "Fecha Venta"], data);
    });
    document.getElementById("txtnombre").value = "";
}

function listarCombos() {
    $.get("VentaMaq/listarCombustible", function (data) {
        llenarCombo(data, document.getElementById("cboxCombustible"), true)

    });
    $.get("VentaMaq/listarTipoMaq", function (data) {
        llenarCombo(data, document.getElementById("cboxTipoMaq"), true)

    });
    $.get("VentaMaq/listarEstadoMaq", function (data) {
        llenarCombo(data, document.getElementById("cboxEstado"), true)

    });
}

listarCombos();

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

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-ventamaq' class='table'>";
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < arrayColumnas.length; i++) {
        contenido += "<td>";
        contenido += arrayColumnas[i];
        contenido += "</td>";
    }
    contenido+="<td>Acciones</td>"
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
    $("#tabla-ventamaq").dataTable({
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

function abrirModal(id) {
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }
    if (id == 0) {
        borrarDatos();
    } else {
        $.get("VentaMaq/mostrarInfo/?id=" + id, function (data) {
            document.getElementById("txtId").value = data[0].Id;
            document.getElementById("txtNombre").value = data[0].Nombre;
            document.getElementById("txtColor").value = data[0].Color;
            document.getElementById("txtPeso").value = data[0].Peso;
            document.getElementById("txtMarca").value = data[0].Marca;
            document.getElementById("txtModelo").value = data[0].Modelo;
            document.getElementById("txtPrecio").value = data[0].Precio;
            document.getElementById("cboxCombustible").value = data[0].CombustibleId;
            document.getElementById("cboxTipoMaq").value = data[0].TipoMaqId;
            document.getElementById("datepickerVentaMaq").value = data[0].FechaVenta;
            document.getElementById("txtHabilitado").value = data[0].Habilitado;
            document.getElementById("cboxEstado").value = data[0].EstadoId;
            
        });
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

function Agregar() {
    if (datosObligatorios() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtId").value;
        var nombre = document.getElementById("txtNombre").value;
        var color = document.getElementById("txtColor").value;
        var peso = document.getElementById("txtPeso").value;
        var marca = document.getElementById("txtMarca").value;
        var modelo = document.getElementById("txtModelo").value;
        var precio = document.getElementById("txtPrecio").value;
        var combustible = document.getElementById("cboxCombustible").value;
        var tipomaq = document.getElementById("cboxTipoMaq").value;
        var fecha = document.getElementById("datepickerVentaMaq").value;
        var habilitado = document.getElementById("txtHabilitado").value;
        var estado = document.getElementById("cboxEstado").value;

        frm.append("Id", id);
        frm.append("Nombre", nombre);
        frm.append("Color", color);
        frm.append("Peso", peso);
        frm.append("Marca", marca);
        frm.append("Modelo", modelo);
        frm.append("Precio", precio);
        frm.append("CombustibleId", combustible);
        frm.append("TipoMaqId", tipomaq);
        frm.append("FechaVenta", fecha);
        frm.append("Habilitado", habilitado);
        frm.append("EstadoId", estado);

        if (confirm("¿Desea guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "VentaMaq/guardar",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("El dato se guardó correctamente");
                        listar();
                        document.getElementById("btnCancelar").click();
                    } else {
                        if (data == -1) {
                            alert("El dato ya existe");
                        }
                        alert("Ocurrio un error");
                    }
                }
            });
        }
    }
}

function eliminar(id) {
    if (confirm("¿Desea eliminar el dato?") == 1) {
        $.get("VentaMaq/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrió un error");
            } else {
                alert("Se elimino correctamente");
                listar();
            }
        });
    }
}

