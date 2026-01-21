'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useAuthStore } from '@/store/auth.store'
import { StudentProfile } from '@/components/student/StudentProfile'
import { MentorProfile } from '@/components/mentor/MentorProfile'

export default function ProfilePage() {
	const router = useRouter()
	const user = useAuthStore(state => state.user)
	const token = useAuthStore(state => state.token)

	useEffect(() => {
		if (!token) {
			router.push('/login')
			return
		}

		if (user?.role === 'ADMIN') {
			router.push('/admin/profile')
		}
	}, [token, user, router])

	if (!token) return null

	if (user?.role === 'MENTOR') {
		return <MentorProfile />
	}

	return <StudentProfile />
}

