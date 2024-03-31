
(function ($) {
	({
		init: function () {
			var self = this;
			$(function () {
				self.selectOption();

				// self.stepsForm();

				self.slickServices();
				
				self.bindDatePicker();

				self.cloneHtml();

				self.accordion();

				self.readMore();

				self.tabsLink();
			});
		},
		tabsLink: function () {
			$('#tab1').show();
			$('.tab-links li:first').addClass('active');

			$('.tab-links a').on('click', function (e) {
				var currentAttrValue = $(this).attr('href');

				// Hiển thị/Ẩn Tabs
				$('.tabs ' + currentAttrValue).show().siblings().hide();

				// Thay đổi/xóa tab hiện tại để kích hoạt
				$(this).parent('li').addClass('active').siblings().removeClass('active');

				e.preventDefault();
			});
		},

		bindDatePicker: function () {
			if ($("#datetimepicker1").length) {
				$("#datetimepicker1").datepicker({
					dateFormat: "mm/dd/yy",
					duration: "fast",
					beforeShow: function (input, inst) {
						// Move the datepicker to the right of the input
						inst.dpDiv.css({
							marginLeft: input.offsetWidth - 290 + "px",
							marginTop: -input.offsetHeight + 44 + "px"
						});
					}
				});

			}
			if ($("#datetimepicker2").length) {
				$("#datetimepicker2").datepicker({
					dateFormat: "mm/dd/yy",
					duration: "fast",
					beforeShow: function (input, inst) {
						// Move the datepicker to the right of the input
						inst.dpDiv.css({
							marginLeft: input.offsetWidth - 290 + "px",
							marginTop: -input.offsetHeight + 44 + "px"
						});
					}
				});
			}

		},

		stepsForm: function (){
			if ($('#example-form').length){
				var form = $("#example-form");
	
				form.steps({
					headerTag: "h2",
					bodyTag: "section",
					transitionEffect: "fade",
				});
				// $('.steps').find('li').removeClass('disabled')
			}
		},

		selectOption: function () {
			$('.selected-option').click(function () {
				$('.dropdown-list').slideToggle();
			});
			$('.user-login-wrap').click(function () {
				$('.dropdown-user').slideToggle();
			});
			$(document).on('click', function (event) {
				var dropdown = $('.user-login-wrap'); 
				var dropdown2 = $('.selected-option'); 

				var dropdown3 = $('.item-dropdown'); 
				// Kiểm tra xem phần tử được click có thuộc dropdown hay không
				if (!dropdown.is(event.target) && dropdown.has(event.target).length === 0) {
					// Sự kiện click xảy ra bên ngoài dropdown
					// Đóng dropdown ở đây
					$('.dropdown-user').slideUp(); 
				}
				if (!dropdown2.is(event.target) && dropdown2.has(event.target).length === 0) {
					// Sự kiện click xảy ra bên ngoài dropdown
					// Đóng dropdown ở đây
					$('.dropdown-list').slideUp(); 
				}
				if (!dropdown3.is(event.target) && dropdown3.has(event.target).length === 0) {
					// Sự kiện click xảy ra bên ngoài dropdown
					// Đóng dropdown ở đây
					$('.dropdown-content').slideUp();
				}
			});


			function saveSelectedValue(selectedValue, imageUrl) {
				localStorage.setItem('selectedValue', selectedValue);
				localStorage.setItem('selectedImageUrl', imageUrl);
			}

			// Lấy giá trị đã chọn từ localStorage
			function getSelectedValue() {
				return localStorage.getItem('selectedValue');
			}

			// Lấy URL của hình ảnh đã chọn từ localStorage
			function getSelectedImageUrl() {
				return localStorage.getItem('selectedImageUrl');
			}

			var cultureData = '';
			var cultureReturnUrl = $('.custom-dropdown .returnUrl').val();
			$('.dropdown-list span').removeClass('active');
			$('.dropdown-list span').click(function () {
				// Loại bỏ lớp 'active' khỏi tất cả các tùy chọn
				$('.dropdown-list span').removeClass('active');
				// Thêm lớp 'active' cho tùy chọn được chọn
				$(this).addClass('active');

				var selectedValue = $(this).attr('data-value');
				var selectedImageUrl = $(this).find('img').attr('src');
				saveSelectedValue(selectedValue, selectedImageUrl); // Lưu giá trị vào localStorage

				// Cập nhật hình ảnh của tùy chọn đã chọn
				$('#selected-value img').attr('src', selectedImageUrl);
				$('.dropdown-list').slideUp();

				$.ajax({
					type: "POST",
					url: "Home/SetLanguage",
					data: { culture: selectedValue, returnUrl: cultureReturnUrl },
					success: function (result, status, xhr) {
						window.location.href = cultureReturnUrl;
					},
					error: function (xhr, status, error) {

					}
				});
			});

			// Lấy giá trị mặc định từ localStorage
			var defaultOption = getSelectedValue();
			var defaultImageUrl = getSelectedImageUrl();

			// Nếu không có giá trị mặc định, lưu giá trị mặc định và URL của hình ảnh mặc định vào localStorage
			if (!defaultOption) {
				var defaultOptionValue = $('.dropdown-list span.active').attr('data-value');
				var defaultOptionImageUrl = $('.dropdown-list span.active').find('img').attr('src');
				saveSelectedValue(defaultOptionValue, defaultOptionImageUrl);
				$defaultOption.addClass('active');
			}

			// Thiết lập hình ảnh mặc định nếu không có giá trị từ localStorage
			$('#selected-value img').attr('src', defaultImageUrl || '/assets/landing-page/img/logo/unitedStates.webp');

			// Đánh dấu lại tùy chọn đã lưu trong localStorage là active
			var $activeOption = $('.dropdown-list span[data-value="' + defaultOption + '"]');
			$activeOption.addClass('active');


		},

		slickServices: function () {
			if ($('.our-services-slick .slick-slider').length || $('.our-project .slick-slider').length ||  $('.section-partners .slick-slider').length) {
				$('.our-services-slick .slick-slider').slick({
					slidesToScroll: 1,
					arrows: false,
					slidesToShow: 5,
					infinite: true,
					autoplay: true,
					autoplaySpeed: 2000,
					lazyLoad: 'ondemand',
					responsive: [
						{
							breakpoint: 1700,
							settings: {
								slidesToShow: 4.2,
	
							}
						},
						{
							breakpoint: 1400,
							settings: {
								slidesToShow: 3.3,
								infinite: false,
							}
						},
						{
							breakpoint: 1100,
							settings: {
								slidesToShow: 3,
							}
						},
						{
							breakpoint: 1000,
							settings: {
								slidesToShow: 2,
							}
						},
						{
							breakpoint: 640,
							settings: {
								slidesToShow: 1,
								infinite: true,
							}
						},
					]
				});
	
				$('.our-project .slick-slider').slick({
					slidesToScroll: 1,
					arrows: false,
					slidesToShow: 5,
					infinite: true,
					autoplay: true,
					lazyLoad: 'ondemand',
					autoplaySpeed: 2000,
					rtl: true, // Set rtl to true for right-to-left
					responsive: [
						{
							breakpoint: 1700,
							settings: {
								slidesToShow: 4.2,
	
							}
						},
						{
							breakpoint: 1400,
							settings: {
								slidesToShow: 3.3,
								infinite: false,
							}
						},
						{
							breakpoint: 1100,
							settings: {
								slidesToShow: 2.5,
							}
						},
						{
							breakpoint: 1000,
							settings: {
								slidesToShow: 2,
							}
						},
						{
							breakpoint: 640,
							settings: {
								slidesToShow: 1,
								infinite: true,
							}
						},
					]
				});
	
				$('.section-partners .slick-slider').slick({
					slidesToScroll: 1,
					arrows: false,
					slidesToShow: 5,
					infinite: true,
					autoplay: true,
					autoplaySpeed: 2000,
					lazyLoad: 'ondemand',
					responsive: [
						{
							breakpoint: 640,
							settings: {
								slidesToShow: 2,
							}
						},
					]
				});
	
				var slider = $('.slick-slider');
	
				function autoScroll() {
					// Get the current slide index
					var currentSlide = slider.slick('slickCurrentSlide');
	
					// If the current slide is the last one, reset to the first slide
					if (currentSlide === Math.floor(slider.slick('getSlick').options.slidesToShow)) {
						slider.slick('slickGoTo', 0);
					}
				}
				$(window).on('resize load', function () {
					if (window.innerWidth < 1400 && window.innerWidth > 640) {
						setInterval(autoScroll, 2000);
					}
	
				})
			}
			if($('.our-values-list').length){

				$('.our-values-list').slick({
					// Cài đặt của slick ở đây
					dots: false,
					arrows: false,
					infinite: true,
					speed: 300,
					slidesToShow: 1,
					slidesToScroll: 1,
					centerMode: true,
					variableWidth: true,
					responsive: [
						{
							breakpoint: 640,
							settings: {
								slidesToShow: 1,
							}
						},
					]
				});

				$('.slider-btn').click(function(){
					$('.slider-btn').removeClass('active')
					if($(this).hasClass('active')){
						$(this).removeClass('active');
						$(this).parents('.accordion-item').removeClass('isActive')
					}else{
						$(this).addClass('active');
						$(this).parents('.accordion-item').addClass('isActive');
					}
					var slideIndex = $(this).data('slide');
					$('.our-values-list').slick('slickGoTo', slideIndex);
				});
			}
			if ($('.process-slick').length) {

				$('.process-list').slick({
					// Cài đặt của slick ở đây
					dots: false,
					arrows: false,
					infinite: true,
					speed: 300,
					slidesToShow: 1,
					slidesToScroll: 1,
					autoplay: true,
					responsive: [
						{
							breakpoint: 640,
							settings: {
								slidesToShow: 1,
							}
						},
					]
				});

				// Tạo một biến để lưu trữ index của slider hiện tại
				var currentSlideIndex = 0;

				// Tạo một hàm để cập nhật trạng thái của nút slider-btn
				function updateSliderBtn(index) {
					$('.process-slick .slider-btn').removeClass('active');
					$('.process-slick .slider-btn[data-slide="' + index + '"]').addClass('active');
				}

				// Tự động cập nhật trạng thái của nút slider-btn khi slick chuyển slide
				$('.process-list').on('afterChange', function (event, slick, currentSlide) {
					currentSlideIndex = currentSlide;
					updateSliderBtn(currentSlideIndex);
				});

				// Bắt sự kiện click cho nút slider-btn
				$('.process-slick .slider-btn').click(function () {
					var slideIndex = $(this).data('slide');
					$('.process-list').slick('slickGoTo', slideIndex);
					updateSliderBtn(slideIndex);
				});

				// Khởi tạo trạng thái ban đầu cho nút slider-btn
				updateSliderBtn(currentSlideIndex);
			}
			if ($('.project-pictures-slick').length) {

				$('.project-pictures-slick').slick({
					// Cài đặt của slick ở đây
					dots: false,
					arrows: true,
					infinite: true,
					speed: 300,
					slidesToShow: 5,
					slidesToScroll: 1,
					responsive: [
						{
							breakpoint: 1100,
							settings: {
								slidesToShow: 3,
							}
						},
						{
							breakpoint: 640,
							settings: {
								slidesToShow: 1,
							}
						},
					]
				});
			}
		},

		cloneHtml: function () {

			$('.btn-clone a').on('click',function(){
				var parent = $(this).parents('.wrap-inner')
				var cloneHtml = parent.find('.clone-html').children().clone();
				// count div have class .clone-row is child of div have class .wrap-inner
				var newNumber = parent.find('.clone-row').length - 1;

				cloneHtml.find('select').each(function () {
                    var name = $(this).attr('name');
                    if (name !== undefined && name.includes('[x]')) {
						var newName = name.replace('[x]', '[' + newNumber + ']');
                        $(this).attr('name', newName);
                    }
                });

				cloneHtml.find('input, textarea').each(function () {
					var name = $(this).attr('name');
					if (name !== undefined && name.includes('[x]')) {
						var newName = name.replace('[x]', '[' + newNumber + ']');
						$(this).attr('name', newName);
					}
				});

				// find and delete span delete-row other in parent .append
				parent.find('.append').find('.delete-row').remove();
				//var wrappedContent = $("<div class='form row'></div>").append(cloneHtml);
				// find last .append and add cloneHtml
				parent.find('.append').last().append(cloneHtml);
				// $(this).parent().prepend(cloneHtml);
				// wrappedContent.insertBefore($(this).parent());
				$('.delete-row').on('click', function () {
					$(this).parent().remove();
				})
				$('div.select-styled').off('click').on('click', function (e) {
					e.stopPropagation();
					// Đóng tất cả các dropdown khác (nếu có)
					$('div.select-styled.active').not(this).each(function () {
						$(this).removeClass('active').next('ul.select-options').hide();
					});
					$(this).toggleClass('active').next('ul.select-options').toggle();
				});

				// Xử lý sự kiện khi chọn tùy chọn trong dropdown
				$('ul.select-options li').off('click').on('click', function (e) {
					e.stopPropagation();
					var selectedOption = $(this).text();
					var selectStyled = $(this).closest('div.select').find('div.select-styled');
					selectStyled.text(selectedOption);
					selectStyled.removeClass('active');
					$(this).closest('ul.select-options').hide();

					// Điều chỉnh giá trị của trường input ẩn (nếu cần)
					var selectedValue = $(this).attr('rel');
					var inputField = $(this).closest('div.select').find('select');
					inputField.val(selectedValue).trigger('change'); // Trigger sự kiện change nếu cần
				});

				$(document).click(function () {
					$('div.select-styled.active').removeClass('active');
					$('ul.select-options').hide();
				});
			});

		},

		accordion: function () {
			var accordionHeaders = $('.accordion-header');
			accordionHeaders.click(function () {
				if($(this).hasClass('active')){
					$(this).removeClass('active')
				}else{
					$(this).addClass('active')
				}
				if (!$(this).next().is(':visible')) {
					$(this).next().slideDown();
				} else {
					$(this).next().slideUp();
				}
			});
		},

		readMore: function () {
			if($('.read-more-content').length){
				var heightRemore = $('.read-more-content').innerHeight()
				if(heightRemore > 170){
					$('#btnSeeMore').show();
				}
			}
			$('#btnSeeMore').on('click',function(){
				$('#textContainer').addClass('textContainer');
				$('#btnSeeMore').hide();
			})
		}

	}.init());

	setTimeout(() => {
		if ($('.wow').length) {
			new WOW().init();
		}
	}, 1900);

	$('.navbar-toggler').on('click', function () {
		if ($('body').hasClass('openMenu')) {
			$('body').removeClass('openMenu')
		} else {
			$('body').addClass('openMenu')
		}

	});

	$(window).on("load", function () {
		$(".loading-block").delay(1400).fadeOut("slow"); //ローディング画面を1.5秒（1500ms）待機
		$(".loading-block_logo").delay(1300).fadeOut("slow"); //ロゴを1.2秒（1200ms）待機
		 //ここから ローディングエリア（loadingエリア）を2.2秒でフェードアウト後に動かしたいJSを記述
		 if($(".loadingbg").length){
			 $(".loadingbg").delay(500).fadeOut('slow',function(){//ローディングエリア（loadingエリア）を2.2秒でフェードアウト
			 
				$('body').addClass('active');//フェードアウト後bodyにactiveクラス付与
		  
			});
		 }
	});

	

}(jQuery));


$(document).ready(function () {
	$(".projects-list .item-dropdown span").on('click', function () {
		if ($(this).hasClass('active')) {
			$(this).removeClass('active');
			$(this).parents('.item-dropdown').find('.dropdown-content').slideUp();
		} else {
			$(this).parents('.item-dropdown').find('.dropdown-content').slideDown();
			$(this).addClass('active');
		}
	})

	$(".keypress").on('keypress',function(event){
		// Lấy mã Unicode của ký tự
		var charCode = event.which;
	
		// Kiểm tra xem ký tự có phải là số không (0-9)
		if (charCode < 48 || charCode > 57) {
		  // Nếu không phải số, ngăn chặn sự kiện mặc định
		  event.preventDefault();
		}
	  });

	  $('.eye_icon').on('click', function () {
		  var passwordField = $(this).parent().find('input')
		var fieldType = passwordField.attr('type');

		if (fieldType === 'password') {
			passwordField.attr('type', 'text');
			$(this).addClass('active');
		} else {
			passwordField.attr('type', 'password');
			$(this).removeClass('active');
		}
	  });
	$('.eye_icon_confirm_password').on('click', function () {
		var passwordField = $('#ConfirmPassword');
		var fieldType = passwordField.attr('type');

		if (fieldType === 'password') {
			passwordField.attr('type', 'text');
			$(this).addClass('active');
		} else {
			passwordField.attr('type', 'password');
			$(this).removeClass('active');
		}
	});

	function processSteps() {
		if ($('.your-projects-calculate .projects-list').length){
			var ariaLabelledByIdValue = $("#example-form .setup-content.current").attr("aria-labelledby");
			$('.your-projects-calculate .projects-list .projects-item').each(function () {
				var numberPageValue = parseInt($(this).find('.number-page').text());
				var percentItem = ((numberPageValue) / 11) * 100; 
				$(this).find('.percent').text(Math.ceil(percentItem));
				$(this).find('.process-blue').css('width', percentItem + '%');
			});
		}
	}
	
	processSteps();


	// Check bỏ after sau button nếu rớt hàng
	arrangeLastStepAfter();

	$("div.setup-panel div a.btn-primary").trigger("click");
	$(window).resize(function () {
		arrangeLastStepAfter();
	});

	function arrangeLastStepAfter() {
		var rowWidth = $(".steps").width();
		var totalWidth = 0;

		$(".steps ul li").each(function (index) {
			totalWidth += $(this).outerWidth(true) + 18;
			// console.log(index, $(this), totalWidth)
			if (totalWidth > rowWidth) {
				// Nếu vượt quá chiều rộng của hàng, ẩn after của button trước đó
				$(this).prev().addClass("last-step");
				totalWidth = $(this).outerWidth(true);
			} else {
				$(this).prev().removeClass("last-step");
			}
		});
	}	
});
