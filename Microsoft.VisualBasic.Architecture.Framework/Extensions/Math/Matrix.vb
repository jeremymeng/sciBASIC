'     Copyright Jean-Francois Larcher-Pelland 2013-2014
' Distributed under the Boost Software License, Version 1.0.
'    (See accompanying file LICENSE_1_0.txt or copy at
'          http://www.boost.org/LICENSE_1_0.txt)

Namespace Mathematical

    ''' <summary>
    ''' This class contains methods that perform mathematical operations on matrices.
    ''' Operations currently supported are matrix multiplication and scalar multiplication.
    ''' 
    ''' @author Jean-Francois Larcher-Pelland
    ''' </summary>
    Public Module MatrixExtensions

        ''' <summary>
        ''' Multiplies matrices <i>a</i> and <i>b</i> using the brute-force algorithm.
        ''' </summary>
        ''' <param name="a"> The matrix on the left. </param>
        ''' <param name="b"> The matrix on the right. </param>
        ''' <returns> The product of the two matrices. </returns>
        Public Function MatrixMult(a As Double()(), b As Double()()) As Double()()
            If a Is Nothing OrElse b Is Nothing Then
                Throw New ArgumentException("One or both input matrices are null")
            ElseIf a(0).Length <> b.Length Then
                Throw New ArgumentException("Column count of left matrix not equal to row count of right matrix.")
            End If
            Dim row As Integer = a.Length
            Dim col As Integer = b(0).Length
            Dim inner As Integer = b.Length
            Dim out As Double()() = MAT(Of Double)(row, col)
            For i As Integer = 0 To row - 1
                For j As Integer = 0 To col - 1
                    For k As Integer = 0 To inner - 1
                        out(i)(j) += a(i)(k) * b(k)(j)
                    Next
                Next
            Next
            Return out
        End Function

        ''' <summary>
        ''' Performs a scalar multiplication on matrix <i>a</i> using scalar value <i>b</i>.
        ''' </summary>
        ''' <param name="a"> The matrix to be multiplied. </param>
        ''' <param name="b"> Scalar value used in the multiplication. </param>
        ''' <returns> The result of the scalar multiplication. </returns>
        Public Function ScalarMult(a As Double()(), b As Double) As Double()()
            If a Is Nothing Then
                Throw New ArgumentException("Input matrix is null")
            End If
            Dim out As Double()() = MAT(Of Double)(a.Length, a(0).Length)
            For i As Integer = 0 To out.Length - 1
                For j As Integer = 0 To out(0).Length - 1
                    out(i)(j) = a(i)(j) * b
                Next
            Next
            Return out
        End Function
    End Module
End Namespace