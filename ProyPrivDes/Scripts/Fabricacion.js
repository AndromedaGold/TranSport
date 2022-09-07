listar();
function listar() {
    $.get("Fabricacion/listarFabricacion", function (data) {
        crearListado(["ID","Nombre","Color","Cantidad Materia Prima","Cantidad","Fecha de Fabricacion"], data);
    })
}

$("#datepickerFabricacion").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

var nombrePi = document.getElementById("txtnombre");
nombrePi.onkeyup = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("Fabricacion/buscarFabricacion/?nombrePi=" + nombre, function (data) {
        crearListado(["ID", "Nombre", "Color", "Cantidad Materia Prima", "Cantidad", "Fecha de Fabricacion"], data);
    })
}

$(document).ready(function () {

    //Boton ver reporte
    $("#btnMostrarPDF").on("click", function () {
        $("#RptFabricacion").attr("src", "Fabricacion/RptFabricacion").load();
    });

    //Boton descargar reporte
    $("#btnDescargarPDF").on("click", function () {
        window.open("Fabricacion/RptFabricacion")
    });

});

$.get("Fabricacion/listarTipoPiso", function (data) {
    llenarCombo(data, document.getElementById("cboxTipoPiso"), true)
})

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
    contenido += "<table id='tabla-fabricacion' class='table'>";
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
    $("#tabla-fabricacion").dataTable({
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
        $.get("Fabricacion/mostrarInfo/?id=" + id, function (data) {
            document.getElementById("txtId").value = data[0].Id;
            document.getElementById("txtNombre").value = data[0].Nombre;
            document.getElementById("cboxTipoPiso").value = data[0].TipoPisoId;
            document.getElementById("txtColor").value = data[0].Color;
            document.getElementById("txtCantMatPrim").value = data[0].CantidMaterPrim;
            document.getElementById("txtCantidadFabri").value = data[0].CantidadFabri;
            document.getElementById("datepickerFabricacion").value = data[0].Fecha;
            document.getElementById("txtHabilitado").value = data[0].Habilitado;
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
        var id = document.getElementById("txtId").value;
        var nombre = document.getElementById("txtNombre").value;
        var tipo = document.getElementById("cboxTipoPiso").value;
        var color = document.getElementById("txtColor").value;
        var cantmate = document.getElementById("txtCantMatPrim").value;
        var cantfab = document.getElementById("txtCantidadFabri").value;
        var fecha = document.getElementById("datepickerFabricacion").value;
        var habilitado = document.getElementById("txtHabilitado").value;

        frm.append("Id", id);
        frm.append("Nombre", nombre);
        frm.append("TipoPisoId", tipo);
        frm.append("Color", color);
        frm.append("CantidMaterPrim", cantmate);
        frm.append("CantidadFabri", cantfab);
        frm.append("Fecha", fecha);
        frm.append("Habilitado", habilitado);

        if (confirm("¿Dese guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "Fabricacion/guardar",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("El dato se guardo correctamente");
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
        $.get("Fabricacion/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrio un error");
            } else {
                alert("El dato se elimino correctamente");
                listar();
            }
        });
    }
}
