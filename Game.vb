Imports SDL2.SDL

Module Game

    Dim pipes As List(Of SDL_FRect) = New List(Of SDL_FRect)

    Public Const WIDTH = 480
    Public Const HEIGHT = 700
    Public Const PIPE_MOVE_SPEED = 0.1
    Public Const PIPE_THICKNESS = 60

    Dim fullScreenRect As SDL_Rect
    Dim score = 0
    Dim pipeMoveSpeed = PIPE_MOVE_SPEED
    Dim pipeMoving = False

    Dim passedPipe = False
    Dim scoreAdded = False

    Dim g As IntPtr
    Dim window As IntPtr

    Dim steven As Bird = New Bird()

    Enum GameState
        Menu = 0
        Play
        GameOver
    End Enum

    Public state As GameState

    Sub Draw()
        ' Draw background
        If Not state = GameState.GameOver Then
            SDL_SetRenderDrawColor(g, 40, 157, 215, 255)
        Else
            ' Signal something to the player its game over, lets say an orange bg.
            SDL_SetRenderDrawColor(g, 235, 127, 7, 255)
        End If

        SDL_RenderFillRect(g, fullScreenRect)

        DrawPipes()
        For Each rect As SDL_FRect In pipes
            SDL_SetRenderDrawColor(g, 0, 255, 0, 255)
            SDL_RenderFillRectF(g, rect)
        Next
        steven.Draw(g)

        SDL_RenderPresent(g)
    End Sub

    Sub DrawPipes()

        ' for each pipe
        For i = 0 To pipes.Count - 1
            ' you can't replace elements with VB.net lists. you can only remove and add new ones.
            ' we want to replace our rects moved forward one bit.
            ' TODO: make this an array instead. we will only ever have two pipes anyway.
            Dim r = pipes.ElementAt(i)
            pipes.RemoveAt(i)

            ' move pipe to the left
            If pipeMoving Then
                r.x -= pipeMoveSpeed
            End If

            ' have we passed it?
            If r.x < steven.rect.x And passedPipe = False Then
                passedPipe = True
                ' we can't add the score again until we reach the other end of the screen
            End If

            ' are we at the other end of the screen
            If r.x < -r.w Then
                ' if so, move it back to the right
                r.x = WIDTH
                ' we haven't passed this pipe now, its on the far right side. neither have we added a point
                passedPipe = False
                scoreAdded = False
            End If

            ' replace the moved pipe
            pipes.Add(r)
        Next
        SDL_SetWindowTitle(window, "JM Bird - Score: " + CStr(score))
    End Sub

    Sub CheckPipeCollision()
        ' retrieve an SDL_Rect from SDL_FRect of the two pipes, and the bird (No conversion function)
        Dim a As SDL_Rect
        a.x = pipes(0).x
        a.y = pipes(0).y
        a.w = pipes(0).w
        a.h = pipes(0).h
        Dim b As SDL_Rect
        b.x = pipes(1).x
        b.y = pipes(1).y
        b.w = pipes(1).w
        b.h = pipes(1).h
        Dim birdRect As SDL_Rect
        birdRect.x = steven.rect.x
        birdRect.y = steven.rect.y
        birdRect.w = steven.rect.w
        birdRect.h = steven.rect.h

        ' we need to convert SDL_FRect -> SDL_RECT to use SDL_HasIntersection.  
        If SDL_HasIntersection(a, birdRect) Or SDL_HasIntersection(b, birdRect) Or steven.rect.y > HEIGHT Then
            ' Ouch, we hit a pipe
            If state <> GameState.GameOver Then
                ' if its not already game over, make it game over. Don't game over again if its already game over.
                Console.WriteLine("Game Over")
                state = GameState.GameOver
                pipeMoving = False
            End If

            'Reset()
        End If
    End Sub

    Sub Reset()
        ' TODO:
        pipes = New List(Of SDL_FRect)
        Dim r1 As SDL_FRect
        ' start off screen
        r1.x = WIDTH
        r1.y = 0
        r1.w = PIPE_THICKNESS
        r1.h = 300
        pipes.Add(r1)

        Dim r2 As SDL_FRect
        ' start off screen
        r2.x = WIDTH
        r2.y = r1.h + 160
        r2.w = PIPE_THICKNESS
        r2.h = HEIGHT / 2
        pipes.Add(r2)


        steven.Reset()
    End Sub

    Sub ProccessKeys(ByRef e As SDL_Event)
        ' menu
        If state = GameState.Menu Then
            Select Case e.key.keysym.sym

                Case SDL_Keycode.SDLK_SPACE
                    ' START GAME HERE
                    state = GameState.Play
                    score = 0
                    pipeMoving = True
                    steven.Start()
            End Select
        End If

        ' in game
        If state = GameState.Play Then
            Select Case e.key.keysym.sym

                Case SDL_Keycode.SDLK_SPACE
                    steven.momentum = 0.25
            End Select
        End If

        If state = GameState.GameOver Then
            Select Case e.key.keysym.sym

                Case SDL_Keycode.SDLK_SPACE
                    ' MENU START HERE
                    Console.WriteLine("menu")
                    state = GameState.Menu
                    Reset()
            End Select
        End If

    End Sub

    Sub Main()
        state = GameState.Menu

        SDL_Init(SDL_INIT_VIDEO)

        window = SDL_CreateWindow("JM Bird - Score: " + CStr(score), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                WIDTH, HEIGHT, SDL_WindowFlags.SDL_WINDOW_SHOWN)

        fullScreenRect.x = 0
        fullScreenRect.y = 0
        fullScreenRect.w = WIDTH
        fullScreenRect.h = HEIGHT

        Dim e As SDL_Event

        g = SDL_CreateRenderer(window, 0, SDL_RendererFlags.SDL_RENDERER_ACCELERATED)

        Reset()




        Dim running As Boolean = True
        While running
            While SDL_PollEvent(e)
                Select Case e.type
                    Case SDL_EventType.SDL_QUIT
                        running = False
                    Case SDL_EventType.SDL_KEYDOWN
                        ProccessKeys(e)

                    Case Else

                End Select
            End While
            ' do events
            steven.Move()
            CheckPipeCollision()
            If passedPipe And Not scoreAdded Then
                scoreAdded = True
                score += 1
            End If
            ' render
            Draw()
        End While





        SDL_DestroyWindow(window)
        SDL_Quit()
    End Sub

End Module