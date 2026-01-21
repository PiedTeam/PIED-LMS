'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useAuthStore } from '@/store/auth.store'
import { StudentDashboard } from '@/components/student/StudentDashboard'
import { MentorDashboard } from '@/components/mentor/MentorDashboard'

export default function DashboardPage() {
	const router = useRouter()
	const user = useAuthStore(state => state.user)
	const token = useAuthStore(state => state.token)

	useEffect(() => {
		if (!token) {
			router.push('/login')
			return
		}

		if (user?.role === 'ADMIN') {
			router.push('/admin')
		}
	}, [token, user, router])

	if (!token) return null

	if (user?.role === 'MENTOR') {
		return <MentorDashboard />
	}

	return <StudentDashboard />
}

