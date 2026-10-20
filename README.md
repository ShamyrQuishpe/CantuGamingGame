# CantuGaming

Prototipo inicial de Cantuña: La Última Piedra en Unity 2D. Esta primera escena es una plaza sencilla inspirada en el Quito colonial. El personaje es un cuadrado que se mueve por la plaza.

## Abrir y jugar

1. En Unity Hub, añade esta carpeta como proyecto y ábrela con Unity 6000.6.2f1.
2. Abre Assets/CantuGaming/Scenes/Plaza.unity.
3. Pulsa Play y haz clic en la vista Game si el teclado no responde.
4. Usa W, A, S, D o las flechas para mover el cuadrado.

## Cómo está organizado el mapa

- Suelo: adoquines repetidos en un Tilemap.
- Muros y jardineras: iglesia, bordes y tres jardineras con colisiones 2D.
- Decoracion: dos faroles sin colisión.
- Jugador - cuadrado WASD: SpriteRenderer, Rigidbody2D, BoxCollider2D y SquareMover.

Los dibujos son piezas PNG de 16 por 16 píxeles. La escena mantiene la distribución de la referencia sencilla generada para el proyecto. Todavía no incluye enemigos, piedra, cronómetro ni potenciadores.

## Colaborar con GitHub

Se versionan Assets, Packages y ProjectSettings. La carpeta Library y los archivos temporales se reconstruyen en cada equipo y están excluidos por .gitignore. Cada colaborador debe abrir la carpeta del proyecto en Unity Hub con una versión compatible de Unity.
