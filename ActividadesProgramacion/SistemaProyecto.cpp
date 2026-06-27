#include <iostream>
#include <cstring>

using namespace std;

struct Jugador
{
    int idJugador;
    char nombreUsuario[50];
    int nivel;
    int partidasGanadas;
};


void actualizarJugador(Jugador* jugador)
{
    jugador->nivel += 1;
    jugador->partidasGanadas += 1;

    strcpy(jugador->nombreUsuario, "JugadorActualizado");
}


int main()
{
    Jugador jugador1 = {1, "Lucas", 10, 5};

    cout << "=== ANTES ===" << endl;
    cout << "Usuario: " << jugador1.nombreUsuario << endl;
    cout << "Nivel: " << jugador1.nivel << endl;
    cout << "Victorias: " << jugador1.partidasGanadas << endl;


    actualizarJugador(&jugador1);


    cout << "\n=== DESPUES ===" << endl;
    cout << "Usuario: " << jugador1.nombreUsuario << endl;
    cout << "Nivel: " << jugador1.nivel << endl;
    cout << "Victorias: " << jugador1.partidasGanadas << endl;


    return 0;
}