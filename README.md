# CantuGaming — Cantuña: La Última Piedra

Prototipo jugable en Unity 2D, con vista superior y una plaza inspirada en el Quito colonial. Es un juego arcade de persecución y recolección: Cantuña debe conservar la última piedra hasta que termine el tiempo, mientras un diablo intenta robársela.

## Objetivo y ciclo principal

- **Acción:** mover a Cantuña por la plaza, esquivar al diablo, recuperar la piedra si la roba y recoger melcochas.
- **Progreso:** ver el tiempo restante, quién tiene la piedra y los puntos acumulados.
- **Recompensa:** cada melcocha da **200 puntos**; si Cantuña tiene la piedra al terminar el tiempo, obtiene la victoria. Los puntos son una meta adicional y no determinan el resultado de la partida.

## Controles

| Acción | Teclas |
| --- | --- |
| Mover a Cantuña | `W`, `A`, `S`, `D` o flechas |
| Reiniciar después de ganar o perder | Botón **Reintentar** |

En Unity o en el navegador, haz clic en la vista del juego si el teclado no responde. El movimiento diagonal se normaliza para mantener una velocidad consistente.

## Reglas de la partida

1. Cantuña comienza con `UltimaPiedra` y el cronómetro empieza en **90 segundos**. La duración se puede cambiar en el Inspector.
2. Mientras Cantuña tiene la piedra, el diablo lo persigue. Si lo toca sin escudo, roba **la misma piedra** y comienza a huir.
3. Cuando el diablo tiene la piedra, Cantuña puede alcanzarlo para recuperarla. En ese momento el diablo vuelve a perseguirlo.
4. Después de cada transferencia hay **0,8 segundos de protección**. Un contacto continuo solo puede producir una transferencia; para que ocurra otra, los personajes deben separarse y volver a tocarse.
5. Al llegar el cronómetro a cero, Cantuña gana si tiene la piedra y pierde si la tiene el diablo. La partida se detiene y **Reintentar** recarga la escena: tiempo, posiciones, puntos y posesión vuelven a su estado inicial.

El diablo usa navegación por las celdas transitables de la plaza. Su persecución y huida comparten el pathfinding BFS, que evita las celdas de muros y los atajos diagonales entre esquinas bloqueadas. Las velocidades de persecución y huida son configurables en el Inspector.

### Melcochas y escudo

- Al comenzar aparecen **tres melcochas** en espacios libres. Cada una recogida suma 200 puntos, reproduce un sonido y se reemplaza por otra. El marcador permanece visible.
- Un **cubito blanco** aparece cerca de Cantuña en una celda libre. Al tocarlo, se activa un círculo de escudo y saltan chispas. El cubito y los efectos se dibujan con formas simples creadas dentro de Unity, sin arte externo adicional.
- El escudo absorbe **un intento de robo**: al tocar al diablo, desaparece con chispas, Cantuña conserva la piedra y el diablo sigue persiguiendo. Ese mismo contacto no puede robarla después de consumir el escudo. Cuando el escudo se gasta, puede aparecer otro cubito para volver a conseguirlo.

## Interfaz y sonido

Durante la partida se muestran los **puntos** arriba a la izquierda, el **tiempo** arriba a la derecha y el **poseedor de la piedra** en el centro superior. El tiempo se resalta en los últimos diez segundos. Los mensajes «¡Te robaron la piedra!» y «¡Piedra recuperada!» acompañan cada transferencia con un destello. Victoria y derrota tienen una pantalla superpuesta al escenario con el botón **Reintentar**.

Los clips están en `Assets/CantuGaming/Audio/`:

| Archivo | Uso |
| --- | --- |
| `fondo.mp3` | Música de fondo durante la partida; se detiene al terminar. |
| `melcocha_sound.mp3` | Suena al recoger una melcocha. |
| `robo_sound.mp3` | Suena cuando la piedra cambia de Cantuña al diablo o viceversa. |

Los volúmenes de música y efectos se ajustan en el componente `GameAudio` del objeto **Sistema de Game Over**.

## Escenario y organización

La escena principal es `Assets/CantuGaming/Scenes/Plaza.unity`. Tiene suelo de adoquines, iglesia, muros, jardineras, faroles, colisiones 2D, Cantuña animado y un diablo animado. Los faroles son decoración; los muros y jardineras bloquean el paso.

| Componente | Responsabilidad |
| --- | --- |
| `SquareMover` | Entrada y movimiento de Cantuña. |
| `StoneFollowAnchor` | Única fuente de verdad de quién lleva `UltimaPiedra` y protección breve entre transferencias. |
| `EnemyStoneTheftOnContact` | Robo, recuperación y bloqueo del robo cuando Cantuña tiene escudo. |
| `EnemyChase2D` y `GridPathfinder2D` | Persecución, huida y rutas por celdas libres. |
| `PlayerShield2D`, `ShieldPickup2D` y `ShieldPickupSpawner` | Escudo visual, cubito coleccionable, chispas y aparición del siguiente cubito. |
| `MelcochaCollectible`, `MelcochaSpawner` y `ScoreManager` | Recogida, generación y puntos. |
| `GameOverManager` y `GameOverDisplay` | Cronómetro, victoria o derrota, HUD y reinicio. |
| `GameAudio` | Música y efectos de recogida y transferencia. |

## Abrir, probar y publicar

1. Añade esta carpeta a Unity Hub y ábrela con **Unity 6000.6.2f1**.
2. Abre `Assets/CantuGaming/Scenes/Plaza.unity` y pulsa **Play**.
3. Recoge una melcocha y comprueba que aumenten los puntos. Toma el cubito blanco, deja que el diablo te toque para probar el escudo, sepárate y vuelve a tocarlo para observar el robo. Alcanza al diablo para recuperar la piedra.
4. Para probar rápido ambos finales, reduce temporalmente `Match Duration Seconds` en **Sistema de Game Over**. Espera con la piedra para ver victoria y deja que el diablo la tenga al llegar a cero para ver derrota. Pulsa **Reintentar** en cada caso.

Para publicar en Itch.io, genera un **build WebGL nuevo** después de los cambios. Los archivos de `BuildWeb/` o `BuildWeb.zip` existentes no se actualizan automáticamente. Si se sube un ZIP para jugar en el navegador, su `index.html` debe quedar en la raíz del ZIP junto a `Build/` y `TemplateData/`. Prueba la versión subida en el navegador antes de compartir el enlace.

## Estado del avance

El ciclo de mover, recolectar, proteger o recuperar la piedra y llegar a un resultado ya está implementado en una plaza. Quedan para etapas posteriores el balance con jugadores, la ampliación de enemigos prevista para el juego final y la validación completa del build WebGL publicado.

## Colaborar con GitHub

Se versionan `Assets`, `Packages` y `ProjectSettings`. `Library` y los archivos temporales se reconstruyen en cada equipo y están excluidos por `.gitignore`. Cada colaborador debe abrir el proyecto con una versión compatible de Unity.
