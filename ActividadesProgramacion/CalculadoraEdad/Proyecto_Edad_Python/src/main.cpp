#include <iostream>

using namespace std;


bool bisiesto(int anio)
{
    return (anio % 4 == 0 && anio % 100 != 0)
        || (anio % 400 == 0);
}


bool fecha_valida(int dia, int mes, int anio)
{
    if(mes < 1 || mes > 12)
        return false;

    int diasMes[] = {31,28,31,30,31,30,
                     31,31,30,31,30,31};

    if(mes == 2 && bisiesto(anio))
        diasMes[1] = 29;

    return dia >= 1 && dia <= diasMes[mes-1];
}


int calcular_edad(
    int dn, int mn, int an,
    int da, int ma, int aa)
{
    int edad = aa - an;

    if(ma < mn || (ma == mn && da < dn))
        edad--;

    return edad;
}


int main()
{
    int dia, mes, anio;

    cout << "Dia nacimiento: ";
    cin >> dia;

    cout << "Mes nacimiento: ";
    cin >> mes;

    cout << "Anio nacimiento: ";
    cin >> anio;


    if(fecha_valida(dia, mes, anio))
    {
        int edad = calcular_edad(
            dia, mes, anio,
            27, 6, 2026
        );

        cout << "Edad: " << edad << endl;
    }
    else
    {
        cout << "Fecha invalida";
    }


    return 0;
}