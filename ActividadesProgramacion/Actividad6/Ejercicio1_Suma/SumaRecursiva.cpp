/* ====================================================================
Programa: SumaRecursiva.cpp
Objetivo: Resolver sumatorias utilizando una función recursiva.
==================================================================== */
#include <iostream> // Librería estándar para entrada/salida por consola
using namespace std; // Evita tener que anteponer std:: en cada comando
// --- PROTOTIPO DE LA FUNCIÓN ---
int sumaRecursiva(int n);
int main() {
int numeroUsuario;
cout << "=====================================================" << endl;
cout << " EJERCICIO 1: SUMATORIA RECURSIVA (LPR 5-3) " << endl;
cout << "=====================================================" << endl;
cout << "Ingrese un numero entero positivo: ";
cin >> numeroUsuario;
// VALIDACIÓN: Evitamos que el usuario ingrese números negativos
if (numeroUsuario < 0) {
cout << "[ERROR] El numero ingresado debe ser positivo." << endl;
return 1; // Termina el programa indicando un fallo
}
// Llamamos a la función recursiva y guardamos el resultado
int resultado = sumaRecursiva(numeroUsuario);
cout << "-> La suma de los primeros " << numeroUsuario << " numeros naturales es: " << resultado << endl;
cout << "=====================================================" << endl;
return 0; // Finalización exitosa
}
// --- DEFINICIÓN DE LA FUNCIÓN RECURSIVA ---
int sumaRecursiva(int n) {
// CASO BASE: Si el número llega a cero, detenemos las llamadas
if (n == 0) {
return 0;
} else {
// LLAMADA RECURSIVA: Sumamos el número actual con el anterior (n-1)
return n + sumaRecursiva(n - 1);
}
}
