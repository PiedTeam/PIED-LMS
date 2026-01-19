"use client"

import { useState } from "react"
import Link from "next/link"
import { Mail, Lock, User, Apple, Chrome } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardHeader } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"

export default function RegisterPage() {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    password: "",
    confirmPassword: "",
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    // Handle register logic here
    console.log("Register:", formData)
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }))
  }

  return (
    <Card className="w-full max-w-md border-zinc-800 bg-zinc-900/50 backdrop-blur-sm">
      <CardHeader className="space-y-4 text-center">
        <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full border border-zinc-700 bg-zinc-800/50">
          <span className="text-2xl font-bold text-white">{"{}"}</span>
        </div>
        <div>
          <h1 className="text-2xl font-semibold text-white">Create Account</h1>
          <p className="mt-2 text-sm text-zinc-400">
            Already have an account?{" "}
            <Link
              href="/login"
              className="font-medium text-blue-400 hover:text-blue-300 transition-colors"
            >
              Sign in
            </Link>
          </p>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <div className="relative">
              <User className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-zinc-400" />
              <Input
                type="text"
                name="name"
                placeholder="Full name"
                value={formData.name}
                onChange={handleChange}
                className="pl-10 border-zinc-700 bg-zinc-800/50 text-white placeholder:text-zinc-500 focus:border-blue-500 focus:ring-blue-500"
                required
              />
            </div>
          </div>
          <div className="space-y-2">
            <div className="relative">
              <Mail className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-zinc-400" />
              <Input
                type="email"
                name="email"
                placeholder="email address"
                value={formData.email}
                onChange={handleChange}
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
                name="password"
                placeholder="Password"
                value={formData.password}
                onChange={handleChange}
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
                name="confirmPassword"
                placeholder="Confirm Password"
                value={formData.confirmPassword}
                onChange={handleChange}
                className="pl-10 border-zinc-700 bg-zinc-800/50 text-white placeholder:text-zinc-500 focus:border-blue-500 focus:ring-blue-500"
                required
              />
            </div>
          </div>
          <Button
            type="submit"
            className="w-full bg-blue-600 text-white hover:bg-blue-700 transition-colors"
          >
            Sign Up
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
