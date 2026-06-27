/* ====================================================================
Programa: IntercambioPunteros.cpp
Objetivo: Intercambiar valores en memoria física usando punteros.
==================================================================== */
#include <iostream>
using namespace std;
// --- PROTOTIPO DE LA FUNCIÓN ---
// Recibe dos punteros a enteros (direcciones de memoria)
void intercambiarValores(int* ptrA, int* ptrB);
int main() {
int variableX = 100;
int variableY = 500;
cout << "=====================================================" << endl;
cout << " EJERCICIO 3: INTERCAMBIO DE PUNTEROS (LPR 5-3) " << endl;
cout << "=====================================================" << endl;
cout << "=== VALORES ORIGINALES ===" << endl;
cout << "Variable X: " << variableX << " (RAM: " << &variableX << ")" << endl;
cout << "Variable Y: " << variableY << " (RAM: " << &variableY << ")" << endl;
// Invocamos la función pasándole las DIRECCIONES de memoria usando el operador '&'
intercambiarValores(&variableX, &variableY);
cout << "\n=== VALORES LUEGO DE LA FUNCION ===" << endl;
cout << "Variable X: " << variableX << " (Ahora tiene el valor de Y)" << endl;
cout << "Variable Y: " << variableY << " (Ahora tiene el valor de X)" << endl;
cout << "=====================================================" << endl;
return 0;
}
// --- DEFINICIÓN DE LA FUNCIÓN ---
void intercambiarValores(int* ptrA, int* ptrB) {
// Creamos una variable temporal para no perder el valor de ptrA al sobreescribirlo
int variableTemporal = *ptrA; // Guardamos el VALOR apuntado por ptrA (100)
*ptrA = *ptrB; // Copiamos el VALOR apuntado por ptrB en el casillero de ptrA (ptrA toma 500)
*ptrB = variableTemporal; // Copiamos el valor temporal en el casillero de ptrB (ptrB toma 100)
}