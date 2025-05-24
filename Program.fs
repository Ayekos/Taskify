open System
open System.IO

type Task = {
    Id: int
    Title: string
    IsDone: bool
}

let mutable tasks: Task list = []
let filePath = "tasks.txt"

let saveTasks () =
    let lines =
        tasks
        |> List.map (fun t -> $"{t.Id}|{t.Title}|{t.IsDone}")
    File.WriteAllLines(filePath, lines)

let loadTasks () =
    if File.Exists(filePath) then
        tasks <-
            File.ReadAllLines(filePath)
            |> Array.choose (fun line ->
                match line.Split('|') with
                | [| id; title; doneStr |] ->
                    Some { Id = int id; Title = title; IsDone = Boolean.Parse(doneStr) }
                | _ -> None
            )
            |> Array.toList
    else
        tasks <- []

let printTasks () =
    if List.isEmpty tasks then
        Console.ForegroundColor <- ConsoleColor.Red
        printfn "No tasks yet."

    else
        for t in tasks do
            let status = if t.IsDone then "[x]" else "[ ]"
            printfn "%d. %s %s" t.Id status t.Title

let addTask title =
    let newId = (if List.isEmpty tasks then 1 else (List.maxBy (fun t -> t.Id) tasks).Id + 1)
    let newTask = { Id = newId; Title = title; IsDone = false }
    tasks <- tasks @ [newTask]
    Console.ForegroundColor <- ConsoleColor.Blue
    printfn "Task added."

let markTaskDone id =
    tasks <-
        tasks
        |> List.map (fun t -> if t.Id = id then { t with IsDone = true } else t)
    Console.ForegroundColor <- ConsoleColor.Green
    printfn "Task marked as done."

let deleteTask id =
    tasks <- tasks |> List.filter (fun t -> t.Id <> id)
    Console.ForegroundColor <- ConsoleColor.Red
    printfn "Task deleted."

let rec mainLoop () =
    Console.ForegroundColor <- ConsoleColor.Green
    printfn "\n== Taskify Console =="
    Console.ForegroundColor <- ConsoleColor.White
    printfn "1. List tasks"
    printfn "2. Add task"
    printfn "3. Mark task as done"
    printfn "4. Delete task"
    printfn "5. Exit"
    Console.ForegroundColor <- ConsoleColor.Yellow
    printf "Choose an option: "

    match Console.ReadLine() with
    | "1" ->
        printTasks()
        mainLoop()
    | "2" ->
        Console.ForegroundColor <- ConsoleColor.White
        printf "Enter task title: "
        let title = Console.ReadLine()
        addTask title
        saveTasks()
        mainLoop()
    | "3" ->
        Console.ForegroundColor <- ConsoleColor.White
        printf "Enter task Number to mark as done: "
        let id = Console.ReadLine() |> int
        markTaskDone id
        saveTasks()
        mainLoop()
    | "4" ->
        Console.ForegroundColor <- ConsoleColor.White
        printf "Enter task Number to delete: "
        let id = Console.ReadLine() |> int
        deleteTask id
        saveTasks()
        mainLoop()
    | "5" ->
        Console.ForegroundColor <- ConsoleColor.White
        printfn "Goodbye!"
        ()
    | _ ->
        Console.ForegroundColor <- ConsoleColor.Red
        printfn "Invalid option."
        mainLoop()

[<EntryPoint>]
let main _ =
    loadTasks()
    mainLoop()
    0
