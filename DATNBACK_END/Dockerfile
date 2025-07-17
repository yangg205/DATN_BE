# Stage 1: Build the application - Giai đoạn 1: Xây dựng ứng dụng
# Sử dụng base image SDK 8.0 từ Microsoft cho quá trình build.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép tệp project (.csproj) và NuGet.config trước.
# Điều này giúp tận dụng Docker cache hiệu quả hơn,
# vì các dependencies thường ít thay đổi hơn mã nguồn.
COPY ["DATNBACK_END.csproj", "./"]
# Nếu bạn có tệp NuGet.config chứa các nguồn package riêng tư hoặc cấu hình khác,
# hãy đảm bảo nó nằm ở thư mục gốc của build context và được sao chép vào đây.
COPY ["NuGet.config", "./"]

# Khôi phục các package NuGet cần thiết cho dự án.
# Lệnh này sẽ tải xuống tất cả các dependencies được khai báo trong .csproj.
RUN dotnet restore "DATNBACK_END.csproj"

# Sao chép toàn bộ mã nguồn còn lại của ứng dụng vào thư mục làm việc.
# Docker sẽ chỉ sao chép những tệp mới hoặc đã thay đổi so với layer trước đó.
COPY . .

# Đặt lại thư mục làm việc là thư mục gốc của project để các lệnh tiếp theo
# có thể tìm thấy mã nguồn chính xác.
WORKDIR "/src"

# Xuất bản ứng dụng ở chế độ Release.
# -c Release: Build ở cấu hình Release.
# -o /app/out: Đặt đầu ra đã xuất bản vào thư mục /app/out trong container.
# --no-restore: Bỏ qua bước khôi phục package vì đã làm ở bước trên.
# /p:UseAppHost=false: Tạo ra một tệp DLL độc lập thay vì một tệp thực thi.
# Heroku sẽ quản lý process, nên tệp DLL là phù hợp.
RUN dotnet publish -c Release -o /app/out --no-restore /p:UseAppHost=false

# Stage 2: Create runtime image - Giai đoạn 2: Tạo image chạy ứng dụng
# Sử dụng base image ASP.NET Runtime 8.0, nó nhẹ hơn SDK image và chỉ chứa
# những gì cần thiết để chạy ứng dụng, giúp giảm kích thước image cuối cùng.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Đặt thư mục làm việc mặc định trong container cho ứng dụng.
WORKDIR /app

# Tạo một người dùng không phải root và chuyển quyền sở hữu thư mục /app cho người dùng này.
# Đây là một thực tiễn tốt về bảo mật để ứng dụng không chạy với quyền root.
RUN adduser --system --group appuser && chown -R appuser:appuser /app

# Sao chép các tệp đã được xuất bản (output) từ stage 'build' vào stage 'final'.
# Các tệp này sẽ thuộc sở hữu của 'appuser' sau khi lệnh chown ở trên.
COPY --from=build /app/out .

# Khai báo cổng mà ứng dụng ASP.NET Core của bạn sẽ lắng nghe.
# Lệnh EXPOSE này chỉ mang tính tài liệu, Heroku sẽ chỉ định cổng qua biến môi trường.
EXPOSE 7102

# Chuyển sang người dùng không phải root trước khi chạy ứng dụng.
USER appuser

# Định nghĩa điểm khởi đầu (ENTRYPOINT) của container.
# Heroku sẽ sử dụng Procfile để chạy lệnh này, và sẽ cấp biến môi trường PORT.
# ENTRYPOINT sẽ được ghi đè bởi lệnh trong Procfile.
# Bạn có thể giữ dòng này hoặc xóa nó vì Procfile sẽ định nghĩa lệnh chạy.
# ENTRYPOINT ["dotnet", "DATNBACK_END.dll"]