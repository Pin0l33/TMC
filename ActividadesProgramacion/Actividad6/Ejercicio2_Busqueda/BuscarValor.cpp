/* ====================================================================
Programa: BuscarValor.cpp
Objetivo: Cargar un array y buscar linealmente un elemento.
==================================================================== */
#include <iostream>
using namespace std;
int main() {
const int TAMANIO = 10; // Definimos el tamaño del arreglo de forma fija
int vectorDatos[TAMANIO] = {12, 45, 78, 90, 122, 5, 18, 33, 67, 99}; // Inicialización directa
int numeroBuscar;
bool encontrado = false; // Variable de bandera (flag)
cout << "=====================================================" << endl;
cout << " EJERCICIO 2: BUSQUEDA EN ARRAYS (LPR 5-3) " << endl;
cout << "=====================================================" << endl;
cout << "Ingrese el numero que desea buscar en la lista: ";
cin >> numeroBuscar;
// Bucle para recorrer los 10 casilleros del arreglo contiguo de memoria
for (int i = 0; i < TAMANIO; i++) {
// Si el valor en la posición actual coincide con la búsqueda
if (vectorDatos[i] == numeroBuscar) {
cout << "[SISTEMA] ¡Encontrado! El numero " << numeroBuscar << " esta en el indice [" << i << "]" << endl;
cout << " Direccion fisica de RAM: " << &vectorDatos[i] << endl;
encontrado = true; // Cambiamos la bandera a verdadero
break; // Salimos del bucle anticipadamente ya que lo encontramos
}
}
// Si terminamos de recorrer el arreglo y la bandera sigue en falso
if (!encontrado) {
cout << "[SISTEMA] El numero " << numeroBuscar << " no se encuentra en la lista." << endl;
}
cout << "=====================================================" << endl;
return 0;
}
