from datetime import date


def bisiesto(anio):
    return (anio % 4 == 0 and anio % 100 != 0) or (anio % 400 == 0)


def fecha_valida(dia, mes, anio):

    if mes < 1 or mes > 12:
        return False

    dias_mes = [31,28,31,30,31,30,31,31,30,31,30,31]

    if mes == 2 and bisiesto(anio):
        dias_mes[1] = 29

    return dia >= 1 and dia <= dias_mes[mes-1]


def calcular_edad(dn, mn, an, da, ma, aa):

    edad = aa - an

    if (ma < mn) or (ma == mn and da < dn):
        edad -= 1

    return edad


dia_n = int(input("Día nacimiento: "))
mes_n = int(input("Mes nacimiento: "))
anio_n = int(input("Año nacimiento: "))

hoy = date.today()

if fecha_valida(dia_n, mes_n, anio_n):

    edad = calcular_edad(
        dia_n, mes_n, anio_n,
        hoy.day, hoy.month, hoy.year
    )

    print("Edad:", edad)

else:
    print("Fecha inválida")