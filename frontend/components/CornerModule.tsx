interface CornerModuleProps {
  position: string
}

export function CornerModule({ position }: CornerModuleProps) {
  return (
    <div className={`absolute ${position} h-16 w-16 rounded border border-zinc-800 bg-zinc-900/50 p-2`}>
      <div className="grid h-full w-full grid-cols-3 gap-1">
        {Array.from({ length: 9 }).map((_, i) => (
          <div
            key={`dot-${i}`}
            className="h-2 w-2 rounded-full bg-blue-500/30"
          />
        ))}
      </div>
    </div>
  )
}
