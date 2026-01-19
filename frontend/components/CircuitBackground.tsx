"use client"

import { CircuitPulse } from "@/components/CircuitPulse"

export function CircuitBackground() {
  return (
    <>
      {/* CircuitPulse components for animated circuit lines */}
      <div className="absolute inset-0 pointer-events-none opacity-20">
        {/* Main center circuit pulse */}
        <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2">
          <CircuitPulse color="#3b82f6" className="h-[60vmin] w-[60vmin]" />
        </div>
        
        {/* Smaller circuit pulses at corners for additional effect */}
        <div className="absolute left-0 top-0">
          <CircuitPulse color="#3b82f6" className="h-[30vmin] w-[30vmin] opacity-30" />
        </div>
        <div className="absolute right-0 top-0">
          <CircuitPulse color="#3b82f6" className="h-[30vmin] w-[30vmin] opacity-30" />
        </div>
        <div className="absolute bottom-0 left-0">
          <CircuitPulse color="#3b82f6" className="h-[30vmin] w-[30vmin] opacity-30" />
        </div>
        <div className="absolute bottom-0 right-0">
          <CircuitPulse color="#3b82f6" className="h-[30vmin] w-[30vmin] opacity-30" />
        </div>
      </div>
    </>
  )
}
