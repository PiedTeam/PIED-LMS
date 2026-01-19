"use client"

import { useState } from "react"
import Link from "next/link"
import { Mail, Lock, Apple, Chrome } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardHeader } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"

export default function LoginPage() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    // Handle login logic here
    // console.log("Login:", { email, password })
  }

  return (
    <Card className="w-full max-w-md border-zinc-800 bg-zinc-900/50 backdrop-blur-sm">
      <CardHeader className="space-y-4 text-center">
        <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full border border-zinc-700 bg-zinc-800/50">
          <span className="text-2xl font-bold text-white">{"{}"}</span>
        </div>
        <div>
          <h1 className="text-2xl font-semibold text-white">Welcome Back</h1>
          <p className="mt-2 text-sm text-zinc-400">
            Don&apos;t have an account yet?{" "}
            <Link
              href="/register"
              className="font-medium text-blue-400 hover:text-blue-300 transition-colors"
            >
              Sign up
            </Link>
          </p>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <div className="relative">
              <Mail className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-zinc-400" />
              <Input
                type="email"
                placeholder="email address"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="pl-10 border-zinc-700 bg-zinc-800/50 text-white placeholder:text-zinc-500 focus:border-blue-500 focus:ring-blue-500"
                required
              />
            </div>
          </div>
          <div className="space-y-2">
            <div className="relative">
              <Lock className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-zinc-400" />
              <Input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="pl-10 border-zinc-700 bg-zinc-800/50 text-white placeholder:text-zinc-500 focus:border-blue-500 focus:ring-blue-500"
                required
              />
            </div>
          </div>
          <Button
            type="submit"
            className="w-full bg-blue-600 text-white hover:bg-blue-700 transition-colors"
          >
            Login
          </Button>
        </form>

        <div className="relative">
          <Separator className="bg-zinc-700" />
          <span className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-zinc-900/50 px-2 text-xs text-zinc-400">
            or
          </span>
        </div>

        <div className="grid grid-cols-3 gap-3">
          <Button
            type="button"
            variant="outline"
            className="aspect-square h-12 w-full border-zinc-700 bg-zinc-800/50 hover:bg-zinc-700/50 p-0"
          >
            <Apple className="h-5 w-5 text-white" />
          </Button>
          <Button
            type="button"
            variant="outline"
            className="aspect-square h-12 w-full border-zinc-700 bg-zinc-800/50 hover:bg-zinc-700/50 p-0"
          >
            <Chrome className="h-5 w-5 text-white" />
          </Button>
          <Button
            type="button"
            variant="outline"
            className="aspect-square h-12 w-full border-zinc-700 bg-zinc-800/50 hover:bg-zinc-700/50 p-0"
          >
            <span className="text-lg font-bold text-white">X</span>
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}
