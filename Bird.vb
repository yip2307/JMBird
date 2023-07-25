Imports SDL2.SDL

Public Class Bird
    Dim x = 0
    Dim y = 0
    Const C_GRAVITY = 0.0004
    Public momentum As Decimal = 0.0
    Dim gravity As Decimal = 0 '.0005
    Public rect As SDL_FRect

    Sub New()
        Reset()
    End Sub

    Public Sub Reset()
        rect.x = (Game.WIDTH / 4) - 32
        rect.y = Game.HEIGHT / 2 - 32
        rect.w = 32
        rect.h = 32
        momentum = 0.0
        gravity = 0
    End Sub

    Public Sub Start()
        gravity = C_GRAVITY
    End Sub

    Public Sub GameOver()
        'gravity = 0.0005
    End Sub

    Public Sub Move()
        rect.y = rect.y - momentum
        momentum -= gravity
    End Sub

    Public Sub Draw(ByRef g As IntPtr)
        SDL_SetRenderDrawColor(g, 255, 0, 0, 255)
        SDL_RenderFillRectF(g, rect)
    End Sub


End Class
