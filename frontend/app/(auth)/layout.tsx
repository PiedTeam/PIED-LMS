import { CircuitBackground } from "@/components/CircuitBackground"
import { CornerModule } from "@/components/CornerModule"

const CORNER_POSITIONS = [
  "top-4 left-4",
  "top-4 right-4",
  "bottom-4 left-4",
  "bottom-4 right-4",
] as const

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <div className="relative min-h-screen w-full overflow-hidden bg-black">
      {/* Circuit board background pattern */}
      <div className="absolute inset-0">
        {/* Corner modules */}
        {CORNER_POSITIONS.map((position) => (
          <CornerModule key={position} position={position} />
        ))}

        {/* Circuit lines connecting modules */}
        <div className="absolute inset-0 pointer-events-none">
          {/* Top horizontal line */}
          <div className="absolute left-20 top-10 h-px w-[calc(100%-10rem)] bg-zinc-800/30" />
          {/* Bottom horizontal line */}
          <div className="absolute bottom-10 left-20 h-px w-[calc(100%-10rem)] bg-zinc-800/30" />
          {/* Left vertical line */}
          <div className="absolute left-10 top-20 h-[calc(100%-5rem)] w-px bg-zinc-800/30" />
          {/* Right vertical line */}
          <div className="absolute right-10 top-20 h-[calc(100%-5rem)] w-px bg-zinc-800/30" />
          {/* Diagonal lines for circuit effect */}
          <div className="absolute left-20 top-10 h-px w-10 rotate-45 origin-left bg-zinc-800/20" />
          <div className="absolute right-20 top-10 h-px w-10 -rotate-45 origin-right bg-zinc-800/20" />
          <div className="absolute bottom-10 left-20 h-px w-10 -rotate-45 origin-left bg-zinc-800/20" />
          <div className="absolute bottom-10 right-20 h-px w-10 rotate-45 origin-right bg-zinc-800/20" />
        </div>

        {/* CircuitPulse components - Client Component */}
        <CircuitBackground />
      </div>

      {/* Main content */}
      <div className="relative z-10 flex min-h-screen items-center justify-center p-4 sm:p-6 md:p-8">
        <div className="w-full max-w-md">
          {children}
        </div>
      </div>
    </div>
  )
}
